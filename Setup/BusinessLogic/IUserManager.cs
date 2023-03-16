using Setup.Models;

namespace Setup.BusinessLogic;

public interface IUserManager
{
    Dictionary<UserModel, int> UserRooms { get; set; }
}