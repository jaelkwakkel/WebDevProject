using Setup.Models;

namespace Setup.BusinessLogic;

public class GameLogic
{
    public BuildingType[,] CreateGameBoard(int size)
    {
        var gameBoard = new BuildingType[size, size];
        for (var i = 0; i < gameBoard.GetLength(0); i++)
        for (var j = 0; j < gameBoard.GetLength(1); j++)
            gameBoard[i, j] = BuildingType.Grass;

        return gameBoard;
    }

    // private UserModel? _firstPlayerPreviousRound;
    //
    // public GameLogic(GameSetup gameSetup)
    // {
    //     GameSetup = gameSetup;
    //     Players = new List<UserModel>();
    // }
    //
    // public List<UserModel> Players { get; set; }
    // public UserModel UserTurnToPlay { get; set; }
    // public bool GameStarted { get; set; }
    // public bool IsFirstRound { get; set; }
    // public bool RoundEnded { get; set; }
    // public GameSetup GameSetup { get; }
    //
    // public int[][] Board { get; set; }
    //
    // public bool MakeMove(string playerConnectionId, int xPosition)
    // {
    //     var player = GetPlayerFromConnectionId(playerConnectionId);
    //     if (playerConnectionId != UserTurnToPlay.ConnectionId)
    //         return false;
    //
    //     var yPos = 0;
    //     for (var i = 0; i < Board[xPosition].Length; i++)
    //     {
    //         yPos = i;
    //         if (Board[xPosition][yPos] == 0) break;
    //     }
    //
    //     //Replace 1 with player-number
    //     Board[xPosition][yPos] = 1;
    //
    //     ChangePlayersTurn();
    //
    //     return true;
    // }
    //
    //
    // public bool StartGame()
    // {
    //     // if (Players.Count != GameSetup.ExpectedNumberOfPlayers)
    //     //     return false;
    //     GameStarted = true;
    //     InitializeNewGame();
    //     return true;
    // }
    //
    // public void InitializeNewGame()
    // {
    //     ChooseFirstRoundPlayer();
    //     RoundEnded = false;
    //     IsFirstRound = true;
    // }
    //
    // private UserModel? GetPlayerFromConnectionId(string playerConnectionId)
    // {
    //     return Players.SingleOrDefault(x => x.ConnectionId == playerConnectionId);
    // }
    //
    // private UserModel? GetNextPlayerFromConnectionId(string playerConnectionId)
    // {
    //     var currentPlayerToPlay = GetPlayerFromConnectionId(playerConnectionId);
    //
    //     var index = Players.IndexOf(currentPlayerToPlay);
    //
    //     if (index == Players.Count - 1)
    //         return Players.First();
    //     return Players[index + 1];
    // }
    //
    // private void ChangePlayersTurn()
    // {
    //     var currentPlayerToPlay = Players.SingleOrDefault(x => x.ConnectionId == UserTurnToPlay.ConnectionId);
    //
    //     var index = Players.IndexOf(currentPlayerToPlay);
    //
    //     UserTurnToPlay = index == Players.Count - 1 ? Players.First() : Players[index + 1];
    // }
    //
    // private void ChooseFirstRoundPlayer()
    // {
    //     _firstPlayerPreviousRound = _firstPlayerPreviousRound == null
    //         ? Players.First()
    //         : //if no previous round has been played, pick the first
    //         // otherwise it is up to the player next to the player who played first on the previous round
    //         GetNextPlayerFromConnectionId(_firstPlayerPreviousRound.ConnectionId);
    //     UserTurnToPlay = _firstPlayerPreviousRound;
    // }
}