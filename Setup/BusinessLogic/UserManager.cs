using Setup.Models;

namespace Setup.BusinessLogic;

public class UserManager : IUserManager
{
    public UserManager()
    {
        UserRooms = new Dictionary<UserModel, int>();
    }

    public Dictionary<UserModel, int> UserRooms { get; set; }
}