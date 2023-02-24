using Setup.Models;
using System.Collections.Concurrent;

namespace Setup.Hubs
{
    public static class UserRepository
    {
        public static ConcurrentDictionary<string, PlayerModel> Users = new(); //{ get; set; }

        //public UserRepository()
        //{
        //Users = new ConcurrentDictionary<string, PlayerModel>();
        //}

        public static PlayerModel? GetUserById(string id)
        {
            if (Users.TryGetValue(id, out PlayerModel? result))
            {
                Console.WriteLine("Return user with id: " + id + " and name " + result.Name);
                return result;
            }
            return null;
        }

        public static void AddUser(PlayerModel user)
        {
            Users.TryAdd(user.ConnectionId, user);
            Console.WriteLine(Users.Count);
        }

        public static PlayerModel? RemoveUser(string id)
        {
            if (Users.TryRemove(id, out PlayerModel? result))
            {
                return result;
            }
            return null;
        }
    }
}