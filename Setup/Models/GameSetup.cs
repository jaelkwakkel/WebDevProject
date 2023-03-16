namespace Setup.Models;

public class GameSetup
{
    // public GameSetup(string id, string password)
    // {
    //     // Id = Guid.NewGuid().ToString();
    //     Id = id;
    //     // ExpectedNumberOfPlayers = expectedNumberOfPlayers;
    //     Password = password;
    // }

    public string Id { get; set; }

    public string Password { get; set; }
    // public int ExpectedNumberOfPlayers { get; set; }
}