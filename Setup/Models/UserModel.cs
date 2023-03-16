namespace Setup.Models;

public class UserModel
{
    public UserModel(string connectionId)
    {
        ConnectionId = connectionId;
        // Name = name;
    }

    public string ConnectionId { get; set; }
    // public string Name { get; set; }
    // public bool LeftGame { get; set; }
}