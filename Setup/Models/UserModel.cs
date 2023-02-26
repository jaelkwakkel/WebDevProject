namespace Setup.Models;

public class UserModel
{
    public UserModel(string connectionId, string name)
    {
        ConnectionId = connectionId;
        Name = name;
    }

    public string ConnectionId { get; set; }
    public string Name { get; set; }
    public bool LeftGame { get; set; }
}