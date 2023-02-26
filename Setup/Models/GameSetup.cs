namespace Setup.Models;

public class GameSetup
{
    public GameSetup(int expectedNumberOfPlayers, string password)
    {
        Id = Guid.NewGuid().ToString();
        ExpectedNumberOfPlayers = expectedNumberOfPlayers;
        Password = password;
    }

    public string Id { get; set; }
    public string Password { get; set; }
    public int ExpectedNumberOfPlayers { get; set; }
}