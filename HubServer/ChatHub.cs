using Microsoft.AspNetCore.SignalR;

namespace HubServer
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = string.Empty;
            var userName = string.Empty;

            userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            userName = Context.GetHttpContext()?.Request.Query["userName"].ToString();

            Console.WriteLine($"User {userName} with Id {userId} has connected.");

            await Clients.Caller.SendAsync("ReceiveSystemMessage", $"Hi {Context.ConnectionId}, you have just connected");
        }
    }
}
