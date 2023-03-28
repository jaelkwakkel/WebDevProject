namespace Setup.Models;

public class UserModel
{
    public UserModel(string connectionId, string name)
    {
        ConnectionId = connectionId;
        Name = name;
        Score = 0;
    }

    public string ConnectionId { get; }
    public int Score { get; set; }
    public string Name { get; }
}