using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Setup.Areas.Identity.Data;
using Setup.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace Setup.Hubs;

public class GameHub : Hub
{
    private static readonly List<GameGroup> Games = new();
    private static readonly List<UserModel> Users = new();

    private readonly UserManager<SetupUser> _userManager;
    private readonly SetupContext _context;

    public GameHub(UserManager<SetupUser> usermanager, SetupContext context)
    {
        _userManager = usermanager;
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        ClaimsPrincipal? loggedInUser = Context.User;
        if (loggedInUser is not null)
        {
            IIdentity? userIdentity = loggedInUser.Identity;
            if (userIdentity is not null)
            {
                string? user = userIdentity.Name;

                if (user is not null)
                {
                    if (Users.All(x => x.ConnectionId != Context.ConnectionId))
                    {
                        Users.Add(new UserModel(Context.ConnectionId, user));
                        await Clients.Caller.SendAsync("HideNameInput");
                    }
                }
            }
        }
        await base.OnConnectedAsync();
    }

    public async Task CreateOrJoin(string key, string name)
    {
        if (Users.All(x => x.ConnectionId != Context.ConnectionId))
        {
            Users.Add(new UserModel(Context.ConnectionId, name));
        }

        if (key.Length > 25)
        {
            await Clients.Caller.SendAsync("GameJoinError", "Key can be no longer than 25 characters");
            return;
        }

        //Key is user-given input. Sanitize it
        var sanitizedKey = new HtmlSanitizer().Sanitize(key);
        if (sanitizedKey.Length < 6)
        {
            await Clients.Caller.SendAsync("GameJoinError", "Key must be at least 6 characters");
            return;
        }

        var user = GetConnectedUser();
        if (user == null) return;
        var game = Games.FirstOrDefault(g => g.Key == sanitizedKey);
        if (game == null)
        {
            game = new GameGroup(sanitizedKey, user); // { Key = sanitizedKey, Owner = user };
            Games.Add(game);
        }

        if (game.HasFinished || game.HasStarted)
        {
            await Clients.Caller.SendAsync("GameJoinError", "You cannot join a group which has already started");
            return;
        }

        game.Users.Add(user);
        await Groups.AddToGroupAsync(user.ConnectionId, game.Key);

        await Clients.Group(game.Key).SendAsync("UpdateUserList", game.GetUsersAsJson());
        await Clients.Group(game.Key).SendAsync("GamePlayMessage", $"{user.Name} has joined the game.");

        await Clients.Caller.SendAsync("JoinedGroup", sanitizedKey);
    }

    private UserModel? GetConnectedUser()
    {
        return Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
    }

    public async Task Start()
    {
        var game = Games.FirstOrDefault(g =>
            g.Owner.ConnectionId == Context.ConnectionId && g is { HasFinished: false, HasStarted: false });

        if (game != null)
        {
            game.HasStarted = true;
            foreach (var item in Users)
            {
                //Start the game with 15 score;
                item.Score = 15;
            }
            await Clients.Group(game.Key).SendAsync("GamePlayMessage", "Game has started.");
        }
        else
        {
            game = Games.FirstOrDefault(g =>
            g is { HasFinished: false, HasStarted: false } &&
            g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

            if (game is null)
            {
                return;
            }
            await Clients.Caller.SendAsync("GamePlayError", "Wait for the owner of this game to start the game.");
        }
    }

    public Task SaveFinishedGameToAccount()
    {
        Console.WriteLine("Saving game...");
        var game = Games.FirstOrDefault(g => g is { HasFinished: true, HasStarted: true } &&
        g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

        if (game is null)
        {
            Console.WriteLine("Save cancelled - game is null");
            return Task.CompletedTask;
        }

        var currentUser = GetConnectedUser();

        if (currentUser is null)
        {
            Console.WriteLine("Save cancelled - current user is null");
            return Task.CompletedTask;
        }

        UserModel winner = game.Users.OrderByDescending(x => x.Score).First();

        GameFinishData gameFinishData = new()
        {
            Score = currentUser.Score,
            WinnerName = winner.Name,
            WonGame = winner == GetConnectedUser()
        };

        SetupUser? setupUser = _userManager.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier);

        if (setupUser is null)
        {
            Console.WriteLine("Save cancelled - setup user is null");
            return Task.CompletedTask;
        }

        setupUser.FinishedGames.Add(gameFinishData);

        if (currentUser.Score > setupUser.highScore)
        {
            setupUser.highScore = currentUser.Score;
        }

        _context.SaveChanges();

        Console.WriteLine("Saved for account: " + setupUser.UserName + " AKA " + currentUser.Name);
        return Task.CompletedTask;
    }

    public async Task PlacedBuilding(string moveValues)
    {
        //TODO: C: May throw error
        var moveValuesObject = JsonConvert.DeserializeObject<MoveValues>(moveValues);

        var game = Games.FirstOrDefault(g =>
            g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

        if (game is null)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Game not found.");
            return;
        }

        if (game.HasStarted == false)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Game has not started yet.");
            return;
        }

        if (game.HasFinished == true)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Game has finished already.");
            return;
        }

        //Check if it is current user's turn to play
        UserModel? currentUser = GetConnectedUser();
        if (currentUser != game.UserTurn)
        {
            await Clients.Caller.SendAsync("GamePlayError", "It is not your turn.");
            return;
        }

        var buildingType = moveValuesObject.buildingType;

        if (game.GetPriceOfBuilding(buildingType) > currentUser.Score)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Not enough money.");
            return;
        }

        var (succeed, errorMessage) =
            game.TryPlaceBuilding(moveValuesObject.xPosition, moveValuesObject.yPosition, buildingType, Context.ConnectionId);
        if (!succeed)
        {
            await Clients.Caller.SendAsync("GamePlayError", errorMessage);
            return;
        }

        currentUser.Score -= game.GetPriceOfBuilding(buildingType);

        int score = game.CalculateScore(Context.ConnectionId);
        currentUser.Score += score;
        //1 free score to prevent getting 0 score with no buildings
        if (currentUser.Score <= 5)
        {
            currentUser.Score++;
        }

        int index = game.Users.IndexOf(currentUser);
        UserModel nextUser = game.Users[0];
        if (index < game.Users.Count - 1)
        {
            nextUser = game.Users[index + 1];
        }
        game.UserTurn = nextUser;

        await Clients.Client(game.UserTurn.ConnectionId).SendAsync("GamePlayMessage", "It is your turn.");

        await Clients.Caller.SendAsync("NewScore", currentUser.Score);
        await Clients.Group(game.Key).SendAsync("UpdateBoard", game.GetBoardAsJsonString());
        await Clients.Group(game.Key).SendAsync("UpdateUserList", game.GetUsersAsJson());

        if (game.CheckGameFinished())
        {
            game.HasFinished = true;
            //Winner is the user with the highest score.
            UserModel winner = game.Users.OrderByDescending(x => x.Score).First();
            await Clients.Group(game.Key).SendAsync("Finishgame", winner.Name);
        }
    }

    private class MoveValues
    {
        public BuildingType buildingType;
        public int xPosition;
        public int yPosition;
    }
}