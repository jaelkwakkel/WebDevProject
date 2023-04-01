using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NuGet.Protocol;
using Setup.Areas.Identity.Data;
using Setup.Models;

namespace Setup.Hubs;

public class GameHub : Hub
{
    private static readonly List<GameGroup> Games = new();
    private static readonly List<UserModel> Users = new();
    private readonly SetupContext _context;

    private readonly UserManager<SetupUser> _userManager;
    private readonly HtmlSanitizer htmlSanitizer;

    public GameHub(UserManager<SetupUser> usermanager, SetupContext context)
    {
        _userManager = usermanager;
        _context = context;
        htmlSanitizer = new HtmlSanitizer();
    }

    public override async Task OnConnectedAsync()
    {
        var loggedInUser = Context.User;
        var userIdentity = loggedInUser?.Identity;
        var user = userIdentity?.Name;

        if (user != null)
            if (Users.All(x => x.ConnectionId != Context.ConnectionId))
            {
                Users.Add(new UserModel(Context.ConnectionId, user));
                await Clients.Caller.SendAsync("HideNameInput");
            }

        await base.OnConnectedAsync();
    }

    public async Task CreateOrJoin(string unsafeKey, string unsafeName)
    {
        var key = htmlSanitizer.Sanitize(unsafeKey);
        var name = htmlSanitizer.Sanitize(unsafeName);

        if (Users.All(x => x.ConnectionId != Context.ConnectionId))
            Users.Add(new UserModel(Context.ConnectionId, name));

        if (key.Length > 25)
        {
            await Clients.Caller.SendAsync("GameJoinError", "Key can be no longer than 25 characters");
            return;
        }

        if (key.Length < 6)
        {
            await Clients.Caller.SendAsync("GameJoinError", "Key must be at least 6 characters");
            return;
        }

        var user = GetConnectedUser();
        if (user == null) return;
        var game = Games.FirstOrDefault(g => g.Key == key);
        if (game == null)
        {
            game = new GameGroup(key, user);
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

        await Clients.Caller.SendAsync("JoinedGroup", key);
    }

    public UserModel? GetConnectedUser()
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
                //Start the game with 15 score;
                item.Score = 15;
            await Clients.Group(game.Key).SendAsync("GamePlayMessage", "Game has started.");
        }
        else
        {
            game = Games.FirstOrDefault(g =>
                g is { HasFinished: false, HasStarted: false } &&
                g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

            if (game is null) return;
            await Clients.Caller.SendAsync("GamePlayError", "Wait for the owner of this game to start the game.");
        }
    }

    public async Task SaveFinishedGameToAccount()
    {
        Console.WriteLine("Saving game...");
        var game = Games.FirstOrDefault(g => g is { HasFinished: true, HasStarted: true } &&
                                             g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

        if (game is null)
        {
            // Specific message for developers
            Console.WriteLine("Save cancelled - game is null");
            // Generic message for users
            // Same method for all error messages
            await Clients.Caller.SendAsync("GamePlayError", "Can not save the game.");
            return;
        }

        var currentUser = GetConnectedUser();

        if (currentUser is null)
        {
            Console.WriteLine("Save cancelled - current user is null");
            await Clients.Caller.SendAsync("GamePlayError", "Can not save the game.");
            return;
        }

        var winner = game.Users.OrderByDescending(x => x.Score).First();

        GameFinishData gameFinishData = new()
        {
            Score = currentUser.Score,
            WinnerName = winner.Name,
            WonGame = winner == GetConnectedUser()
        };

        var setupUser = _userManager.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier);

        if (setupUser is null)
        {
            Console.WriteLine("Save cancelled - setup user is null");
            await Clients.Caller.SendAsync("GamePlayError", "Can not save the game.");
            return;
        }

        setupUser.FinishedGames.Add(gameFinishData);

        if (currentUser.Score > setupUser.HighScore) setupUser.HighScore = currentUser.Score;

        _context.SaveChanges();

        Console.WriteLine("Saved for account: " + setupUser.UserName + " AKA " + currentUser.Name);
    }

    public async Task PlacedBuilding(string unsafeMoveValues)
    {
        var moveValues = htmlSanitizer.Sanitize(unsafeMoveValues);

        //TODO: C: May throw error
        //Do not replace with var!
        var moveValuesObject = JsonConvert.DeserializeObject<MoveValues>(moveValues);

        if (moveValuesObject is null)
        {
            await Clients.Caller.SendAsync("GamePlayError", "An unexpected error ocurred.");
            return;
        }

        var game = Games.FirstOrDefault(g =>
            g.Users.Any(gl => gl.ConnectionId == Context.ConnectionId));

        if (game is null)
        {
            await Clients.Caller.SendAsync("GamePlayError", "An unexpected error ocurred.");
            return;
        }

        if (game.HasStarted == false)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Game has not started yet.");
            return;
        }

        if (game.HasFinished)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Game has finished already.");
            return;
        }

        //Check if it is current user's turn to play
        var currentUser = GetConnectedUser();
        if (currentUser != game.UserTurn)
        {
            await Clients.Caller.SendAsync("GamePlayError", "It is not your turn.");
            return;
        }

        var buildingType = moveValuesObject.BuildingType;

        if (game.GetPriceOfBuilding(buildingType) > currentUser.Score)
        {
            await Clients.Caller.SendAsync("GamePlayError", "Not enough money.");
            return;
        }

        var (succeed, errorMessage) =
            game.TryPlaceBuilding(moveValuesObject.XPosition, moveValuesObject.YPosition, buildingType,
                Context.ConnectionId);
        if (!succeed)
        {
            await Clients.Caller.SendAsync("GamePlayError", errorMessage);
            return;
        }

        currentUser.Score -= game.GetPriceOfBuilding(buildingType);

        var score = game.CalculateScore(Context.ConnectionId);
        currentUser.Score += score;
        //1 free score to prevent getting 0 score with no buildings
        if (currentUser.Score <= 5) currentUser.Score++;

        var index = game.Users.IndexOf(currentUser);
        var nextUser = game.Users[0];
        if (index < game.Users.Count - 1) nextUser = game.Users[index + 1];
        game.UserTurn = nextUser;

        await Clients.Client(game.UserTurn.ConnectionId).SendAsync("GamePlayMessage", "It is your turn.");

        await Clients.Caller.SendAsync("NewScore", currentUser.Score);
        await Clients.Group(game.Key).SendAsync("UpdateBoard", game.GetBoardAsJsonString());
        await Clients.Group(game.Key).SendAsync("UpdateUserList", game.GetUsersAsJson());

        if (game.CheckGameFinished())
        {
            game.HasFinished = true;
            //Winner is the user with the highest score.
            var winner = game.Users.OrderByDescending(x => x.Score).First();
            await Clients.Group(game.Key).SendAsync("Finishgame", winner.Name);
        }
    }

    public async Task RemoveGame(string unsafeRoomCode)
    {
        var roomCode = htmlSanitizer.Sanitize(unsafeRoomCode);

        var setupUser = _userManager.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier);

        if (setupUser is null)
        {
            Console.WriteLine("Remove cancelled - You need to be logged in to remove a game");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        if (!(await _userManager.GetRolesAsync(setupUser)).Contains("moderator"))
        {
            Console.WriteLine("Remove cancelled - You need to be moderator to remove a game");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        var game = Games.FirstOrDefault(g =>
            g.Key.Equals(roomCode));

        if (game is null)
        {
            await Clients.Caller.SendAsync("ErrorMessage", "An unexpected error ocurred.");
            return;
        }

        await Clients.Caller.SendAsync("Message", "Succesfully removed game with code: " + game.Key);
        Games.Remove(game);
        List<(string, bool)> gameInfo = new();
        Games.ForEach(x => { gameInfo.Add((x.Key, x.HasFinished)); });

        await Clients.Caller.SendAsync("UpdateGameList", gameInfo.ToJson());
    }

    public async Task RemoveAllFinishedGames()
    {
        var setupUser = _userManager.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier);

        if (setupUser is null)
        {
            Console.WriteLine("Remove cancelled - You need to be logged in to remove a game");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        if (!(await _userManager.GetRolesAsync(setupUser)).Contains("moderator"))
        {
            Console.WriteLine("Remove cancelled - You need to be moderator to remove a game");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        var amount = Games.RemoveAll(game => game.HasFinished);
        await Clients.Caller.SendAsync("Message", $"Succesfully removed {amount} games");
    }

    public async Task GetActiveGames()
    {
        var setupUser = _userManager.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier);

        if (setupUser is null)
        {
            Console.WriteLine("Remove cancelled - You need to be logged in to view active games");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        if (!(await _userManager.GetRolesAsync(setupUser)).Contains("moderator"))
        {
            Console.WriteLine("Remove cancelled - You need to be moderator to view active games");
            await Clients.Caller.SendAsync("Abort", "An unexpected error ocurred.");
            return;
        }

        List<(string, bool)> gameInfo = new();
        Games.ForEach(x => { gameInfo.Add((x.Key, x.HasFinished)); });

        await Clients.Caller.SendAsync("UpdateGameList", gameInfo.ToJson());
    }


    private class MoveValues
    {
        public readonly BuildingType BuildingType = BuildingType.Grass;
        public readonly int XPosition = 0;
        public readonly int YPosition = 0;
    }
}