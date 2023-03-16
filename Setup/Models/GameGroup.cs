namespace Setup.Models;

public class GameGroup
{
    public bool HasStarted { get; set; }
    public bool HasFinished { get; set; }
    public string Key { get; set; }

    //ConnectionId
    public UserModel Owner { get; set; }
    public List<UserModel> Users { get; set; } = new();
}