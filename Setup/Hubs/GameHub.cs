using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using Setup.BusinessLogic;
using Setup.Models;

namespace Setup.Hubs;

public class GameHub : Hub
{
    private static readonly List<GameManager> _games = new();
    private static readonly List<UserModel> _users = new();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (user == null)
            return;

        _users.Remove(user);

        await base.OnDisconnectedAsync(exception);
    }


    public async Task AddUser(string name)
    {
        UserModel user;
        lock (_users)
        {
            name = Regex.Replace(name, @"\s+", "").ToLower();

            if (name.Length > 10)
                name = name.Substring(0, 10);

            var nameExists = _users.Any(x => x.Name == name);
            if (nameExists)
            {
                var rnd = new Random();
                name = name + rnd.Next(1, 100);
            }

            user = new UserModel(Context.ConnectionId, name);
            _users.Add(user);
        }

        await base.OnConnectedAsync();
    }

    public async Task CreateGame(string password)
    {
        var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (user is null)
        {
            Console.WriteLine("Unknown user");
            return;
        }

        var gameSetup = new GameSetup
        {
            Id = GetRandomId(),
            Password = password
        };
        var game = new GameManager(gameSetup);
        Console.WriteLine("game created with id: " + game.GameSetup.Id);
        _games.Add(game);
        game.Players.Add(user);

        //Debugging
        Console.WriteLine("Available games: ");
        foreach (var gameManager in _games) Console.WriteLine("----->" + gameManager.GameSetup.Id);

        await Clients.Caller.SendAsync("CreatedGame", game.GameSetup.Id);
    }

    public async Task JoinGame(string id, string password)
    {
        var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (user is null)
        {
            Console.WriteLine("Unknown user");
            await Clients.Caller.SendAsync("ErrorOnJoinGame", "Unknown user");
        }

        var game = _games.FirstOrDefault(x => x.GameSetup.Id == id);
        //Check if game exists
        if (game == null)
        {
            await Clients.Caller.SendAsync("ErrorOnJoinGame", "Game does not exist");
            return;
        }

        //Check if password is correct
        if (!string.IsNullOrEmpty(game.GameSetup.Password))
            if (game.GameSetup.Password != password)
            {
                await Clients.Caller.SendAsync("ErrorOnJoinGame", "Incorrect password");
                return;
            }

        Console.WriteLine("game joined with id: " + game.GameSetup.Id);
        game.Players.Add(user);

        //Debugging
        Console.WriteLine("Available games: ");
        foreach (var gameManager in _games) Console.WriteLine("----->" + gameManager.GameSetup.Id);

        await Clients.Caller.SendAsync("JoinedGame");
    }

    public async Task SendMessage(string msg, string gameId)
    {
        Console.WriteLine(Context.ConnectionId);
        Console.WriteLine("Msg: " + msg + " | " + gameId);
        var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
        var allPlayersInTheGame = GetPlayersFromGame(game);
        allPlayersInTheGame.ForEach(Console.WriteLine);
        // await Clients.Group(gameId).SendAsync("ReceiveMessage", msg);
        await Clients.Clients(allPlayersInTheGame).SendAsync("ReceiveMessage", msg);
    }

    private string GetRandomId()
    {
        var random = new Random();
        var randomId = random.Next(0, 999999).ToString("D6");
        while (_games.Any(x => x.GameSetup.Id == randomId))
            randomId = random.Next(0, 999999).ToString("D6");
        return randomId;
    }

    private List<string> GetPlayersFromGame(GameManager game)
    {
        return game.Players.Where(x => !x.LeftGame).Select(y => y.ConnectionId).ToList();
    }
}

// using System.Text.RegularExpressions;
// using Microsoft.AspNetCore.SignalR;
// using Setup.BusinessLogic;
// using Setup.Models;
//
// namespace Setup.Hubs;
//
// public class GameHub : Hub
// {
//     private static readonly List<GameManager> _games = new();
//
//     private static readonly List<UserModel> _users = new();
//
//     public override async Task OnConnectedAsync()
//     {
//         await base.OnConnectedAsync();
//     }
//
//     public override async Task OnDisconnectedAsync(Exception? exception)
//     {
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//         if (user == null)
//             return;
//
//         Console.WriteLine("asdf");
//         // await CleanupUserFromGames();
//         // await CleanupUserFromUsersList();
//
//         await base.OnDisconnectedAsync(exception);
//     }
//
//     public async Task AddUser(string name)
//     {
//         UserModel user;
//         lock (_users)
//         {
//             name = Regex.Replace(name, @"\s+", "").ToLower();
//
//             if (name.Length > 10)
//                 name = name.Substring(0, 10);
//
//             var nameExists = _users.Any(x => x.Name == name);
//             if (nameExists)
//             {
//                 var rnd = new Random();
//                 name = name + rnd.Next(1, 100);
//             }
//
//             user = new UserModel(Context.ConnectionId, name);
//             _users.Add(user);
//         }
//
//         await GetAllPlayers();
//         await Clients.Client(Context.ConnectionId).SendAsync("GetCurrentUser", user);
//         await base.OnConnectedAsync();
//     }
//
//     public async Task GetAllPlayers()
//     {
//         await Clients.All.SendAsync("GetAllPlayers", _users);
//     }
//
//     public async Task UpdateAllGames()
//     {
//         await Clients.All.SendAsync("UpdateAllGames", _games);
//     }
//
//     public async Task CreateGame(int expectedNumberOfPlayers, string id, string password)
//     {
//         await CleanupUserFromGames();
//
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//         var gameSetup = new GameSetup(expectedNumberOfPlayers, id, password);
//
//         var game = new GameManager(gameSetup);
//         game.Players.Add(user);
//         _games.Add(game);
//
//         Console.WriteLine("game started with id: " + gameSetup.Id);
//         Console.WriteLine("Available games: ");
//         foreach (var gameManager in _games) Console.WriteLine("----->" + gameManager.GameSetup.Id);
//
//         await GameUpdated(game);
//         await UpdateAllGames();
//     }
//
//     public async Task UpdateGame(string gameId, int expectedNumberOfPlayers, string password)
//     {
//         var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
//
//         if (game == null) return;
//         if (game.GameStarted) return;
//
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//
//         if (user == null) return;
//         if (game.Players.First() != user) return;
//
//         game.GameSetup.Password = password;
//         game.GameSetup.ExpectedNumberOfPlayers = expectedNumberOfPlayers;
//
//         await GameUpdated(game);
//         await UpdateAllGames();
//     }
//
//     public async Task ExitGame(string gameid)
//     {
//         var game = _games.SingleOrDefault(x => x.GameSetup.Id == gameid);
//
//         if (game == null)
//             return;
//
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//
//         var allPlayersFromGame = GetPlayersFromGame(game);
//
//         if (allPlayersFromGame.Contains(Context.ConnectionId))
//         {
//             var player = game.Players.FirstOrDefault(y => y.ConnectionId == Context.ConnectionId);
//             if (game.GameStarted)
//             {
//                 player.LeftGame = true;
//                 await SendMessageToGameChat(gameid, player.Name, "USER HAS LEFT THE GAME.");
//             }
//             else
//             {
//                 game.Players.Remove(player);
//             }
//         }
//
//         if (game.Players.All(x => x.LeftGame))
//             _games.Remove(game);
//
//         Console.WriteLine("Still existing games after cleanup: ");
//         foreach (var gameManager in _games) Console.WriteLine("----->" + gameManager.GameSetup.Id);
//
//         await GameUpdated(game);
//         await UpdateAllGames();
//         await SendMessageToGameChat(gameid, "Server", $"{user.Name} has left the game.");
//     }
//
//     private async Task SendMessageToGameChat(string gameId, string username, string message)
//     {
//         var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
//         if (game == null)
//             return;
//
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//         if (user == null)
//             return;
//
//         var usersToNotify = GetPlayersFromGame(game);
//
//         await Clients.Clients(usersToNotify).SendAsync("SendMessageToGameChat", message);
//     }
//
//     public async Task StartGame(string gameId)
//     {
//         var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
//         if (game == null) return;
//
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//         if (user == null) return;
//
//         var success = game.StartGame();
//
//         if (success)
//             await GameUpdated(game);
//
//         await UpdateAllGames();
//     }
//
//     public async Task JoinGame(string gameId, string password)
//     {
//         // await CleanupUserFromGamesExceptThisGame(gameId);
//
//         Console.WriteLine("1");
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//         if (user == null) return;
//
//         Console.WriteLine("2");
//         Console.WriteLine("Id = " + gameId + " Existing Id's: ");
//         foreach (var gameManager in _games) Console.WriteLine("----->" + gameManager.GameSetup.Id);
//         var game = _games.SingleOrDefault(x => x.GameSetup.Id == gameId);
//         if (game == null)
//             return;
//
//         Console.WriteLine("3");
//         if (!string.IsNullOrEmpty(game.GameSetup.Password))
//             if (game.GameSetup.Password != password)
//                 return;
//
//         Console.WriteLine("4");
//         if (!game.GameStarted)
//         {
//             if (game.Players.Count == game.GameSetup.ExpectedNumberOfPlayers)
//                 return;
//             game.Players.Add(user);
//         }
//         else
//         {
//             var playerLeftWithThisNickname = game.Players.FirstOrDefault(x => x.LeftGame && x.Name == user.Name);
//
//             if (playerLeftWithThisNickname != null)
//             {
//                 playerLeftWithThisNickname = user;
//                 playerLeftWithThisNickname.LeftGame = false;
//
//                 game.Players.ForEach(u =>
//                 {
//                     if (u.Name == user.Name) u.ConnectionId = user.ConnectionId;
//                 });
//                 if (game.UserTurnToPlay.Name == user.Name)
//                     game.UserTurnToPlay = user;
//                 await SendMessageToGameChat(gameId, "Server", $"{user.Name} has joined the game room.");
//             }
//         }
//
//         Console.WriteLine("5");
//
//         await GameUpdated(game);
//         await UpdateAllGames();
//     }
//
//
//     public async Task MakeMove(string gameId, string move)
//     {
//         var game = _games.SingleOrDefault(x => x.GameSetup.Id == gameId);
//         if (game == null)
//             return;
//         lock (game)
//         {
//             var success = game.MakeMove(Context.ConnectionId, move);
//             if (!success)
//                 return;
//         }
//
//         await GameUpdated(game);
//     }
//
//     public async Task StartNewRound(string gameId)
//     {
//         var game = _games.SingleOrDefault(x => x.GameSetup.Id == gameId);
//         if (game == null)
//             return;
//         game.InitializeNewGame();
//         await GameUpdated(game);
//     }
//
//     // -------------------------------private--------------------
//
//     private async Task GameUpdated(GameManager game)
//     {
//         var allPlayersInTheGame = GetPlayersFromGame(game);
//
//         if (game.GameStarted)
//             foreach (var connectionId in allPlayersInTheGame)
//                 await Clients.Client(connectionId).SendAsync("GameUpdate", game);
//         else
//             await Clients.Clients(allPlayersInTheGame).SendAsync("GameUpdate", game);
//     }
//
//
//     private List<string> GetPlayersFromGame(GameManager game)
//     {
//         return game.Players.Where(x => !x.LeftGame).Select(y => y.ConnectionId).ToList();
//     }
//
//     private async Task CleanupUserFromGames()
//     {
//         var games = _games.Where(x => GetPlayersFromGame(x).Any(y => y == Context.ConnectionId))
//             .ToList();
//
//         foreach (var game in games) await ExitGame(game.GameSetup.Id);
//     }
//
//     private async Task CleanupUserFromGamesExceptThisGame(string gameId)
//     {
//         var games = _games.Where(x =>
//             x.GameSetup.Id != gameId && GetPlayersFromGame(x).Any(y => y == Context.ConnectionId)).ToList();
//
//         foreach (var game in games) await ExitGame(game.GameSetup.Id);
//     }
//
//
//     private async Task CleanupUserFromUsersList()
//     {
//         var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//
//         if (user != null) _users.Remove(user);
//         await GetAllPlayers();
//     }
//
//     public async Task SendMessage(string msg, string gameId)
//     {
//         Console.WriteLine(Context.ConnectionId);
//         Console.WriteLine("Msg: " + msg + " | " + gameId);
//         var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
//         var allPlayersInTheGame = GetPlayersFromGame(game);
//         allPlayersInTheGame.ForEach(Console.WriteLine);
//         await Clients.Clients(allPlayersInTheGame).SendAsync("ReceiveMessage", msg);
//     }
// }
//
// //
// // public async Task SendMessage(string user, string message)
// //     {
// //         await Clients.Group("TestGroup").SendAsync("ReceiveMessage", user, message);
// //         //await Clients.All.SendAsync("ReceiveMessage", user, message);
// //     }
// //
// //     //public Task JoinGroup(string groupName)
// //     //{
// //     //    return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
// //     //}
// //
// //     //private UserRepository _repository;
// //
// //     //public GameHub(UserRepository repository)
// //     //{
// //     //_repository = repository;
// //     //}
// //
// //     public Task Join(string name, string group)
// //     {
// //         var user = UserRepository.GetUserById(Context.ConnectionId);
// //         if (user is not null && user.Group is not null)
// //         {
// //             Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Group);
// //             UserRepository.RemoveUser(Context.ConnectionId);
// //         }
// //
// //         var currentUser = user ?? new UserModel(name, group, Context.ConnectionId);
// //         UserRepository.AddUser(currentUser);
// //         return Groups.AddToGroupAsync(Context.ConnectionId, group);
// //     }
// //
// // public async Task SendMessage(string msg)
// // {
// //     //UserModel? user = _repository.GetUserById(Context.ConnectionId);
// //     var user = UserRepository.GetUserById(Context.ConnectionId);
// //     if (user is not null)
// //     {
// //         Console.WriteLine("Sent message from user: " + user.Name +
// //                           " and message " + msg +
// //                           " to group: " + user.Group);
// //         await Clients.Group(user.Group).SendAsync("ReceiveMessage", user.Name, msg);
// //         //await Clients.All.SendAsync("ReceiveMessage", user.Name, msg);
// //         //await Clients.All.SendAsync("ReceiveMessage", user, message);
// //     }
// //     else
// //     {
// //         Console.WriteLine("User with connectionId: " + Context.ConnectionId + " not found");
// //     }
// //     //Clients.All.chatSent(user.Name, msg);
// // }
// //
// //     //public override Task OnDisconnectedAsync(bool stopCalled)
// //     //{
// //     //    _repository.RemoveUser(Context.ConnectionId);
// //     //    return base.OnDisconnectedAsync(stopCalled);
// //     //}
// // }