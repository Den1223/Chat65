using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Chat65.Hubs // заміни ChatServer на свій namespace
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

