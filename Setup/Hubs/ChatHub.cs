using Microsoft.AspNetCore.SignalR;
using Setup.Models;

namespace Setup.Hubs
{
    public class ChatHub : Hub
    {
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.Group("TestGroup").SendAsync("ReceiveMessage", user, message);
        //    //await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        //public Task JoinGroup(string groupName)
        //{
        //    return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //}

        //private UserRepository _repository;

        //public ChatHub(UserRepository repository)
        //{
        //_repository = repository;
        //}

        public Task Join(string name, string group)
        {
            var user = UserRepository.GetUserById(Context.ConnectionId);
            if (user is not null && user.Group is not null)
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Group);
                UserRepository.RemoveUser(Context.ConnectionId);
            }
            PlayerModel currentUser = new(name, group, Context.ConnectionId);
            //_repository.AddUser(currentUser);
            UserRepository.AddUser(currentUser);
            return Groups.AddToGroupAsync(Context.ConnectionId, group);

            //var users = _repository.Users.ToList();
            //var topic = "Welcome to EmberJS on SignalR";

            //Clients.Caller.lobbyEntered(topic, users);
        }

        public async Task SendMessage(string msg)
        {
            //PlayerModel? user = _repository.GetUserById(Context.ConnectionId);
            PlayerModel? user = UserRepository.GetUserById(Context.ConnectionId);
            if (user is not null)
            {
                Console.WriteLine("Sent message from user: " + user.Name + " and message " + msg + " to group: " + user.Group);
                await Clients.Group(user.Group).SendAsync("ReceiveMessage", user.Name, msg);
                //await Clients.All.SendAsync("ReceiveMessage", user.Name, msg);
                //await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            //Clients.All.chatSent(user.Name, msg);
        }

        //public override Task OnDisconnectedAsync(bool stopCalled)
        //{
        //    _repository.RemoveUser(Context.ConnectionId);
        //    return base.OnDisconnectedAsync(stopCalled);
        //}
    }
}