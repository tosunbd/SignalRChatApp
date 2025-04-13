using ChatContracts;

using Microsoft.AspNetCore.SignalR;

namespace HubServer
{
    public class ChatHub : Hub
    {
        private static readonly object _lockUsers = new object();
        private static List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();
        public override async Task OnConnectedAsync()
        {
            var userId = string.Empty;
            var userName = string.Empty;

            userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            userName = Context.GetHttpContext()?.Request.Query["userName"].ToString();

            lock (_lockUsers)
            {
                //if (_connectedUsers.All(u => u.UserId != userId))
                //{
                    _connectedUsers.Add(new ConnectedUser
                    {
                        UserId = userId,
                        UserName = userName,
                        ConnectionId = Context.ConnectionId
                    });
                //}
            }

            //Console.WriteLine($"User {userName} with Id {userId} has connected.");

            await Clients.Caller.SendAsync("ReceiveSystemMessage", $"Hi {userName}, you have just connected");
            await Clients.All.SendAsync("UpdatedUserList", _connectedUsers);
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUser? user;
            lock (_lockUsers)
            {
                user = _connectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
                if (user != null)
                {
                    _connectedUsers.Remove(user);
                }
            }
            //Console.WriteLine($"User {user.UserName} with Id {user.UserId} has disconnected.");
            if(user is not null)
            {
                // updated the user about the base connection has been changed.
                await Clients.All.SendAsync("UpdatedUserList", _connectedUsers);
            }
            await base.OnDisconnectedAsync(exception);
            //await Clients.All.SendAsync("ReceiveSystemMessage", $"User {user?.UserName} has left the chat");
        }

    }
}
