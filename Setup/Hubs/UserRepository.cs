// using System.Collections.Concurrent;
// using Setup.Models;
//
// namespace Setup.Hubs;
//
// public static class UserRepository
// {
//     public static ConcurrentDictionary<string, UserModel> Users = new(); //{ get; set; }
//
//     //public UserRepository()
//     //{
//     //Users = new ConcurrentDictionary<string, UserModel>();
//     //}
//
//     public static UserModel? GetUserById(string id)
//     {
//         if (Users.TryGetValue(id, out var result))
//         {
//             Console.WriteLine("Return user with id: " + id + " and name " + result.Name);
//             return result;
//         }
//
//         return null;
//     }
//
//     public static void AddUser(UserModel user)
//     {
//         Users.TryAdd(user.ConnectionId, user);
//         foreach (var (key, value) in Users)
//             Console.WriteLine("Key = {0}, Value = {1}, Value = {2}, Value = {3}", key, value.Name, value.Group,
//                 value.ConnectionId);
//         Console.WriteLine(Users.Count);
//     }
//
//     public static UserModel? RemoveUser(string id)
//     {
//         if (Users.TryRemove(id, out var result)) return result;
//         return null;
//     }
// }

