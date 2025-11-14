using Chat65.Data;
using Chat65.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat65.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            var msg = new Message
            {
                User = user,
                Text = message,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinChat(string username)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user == null)
            {
                _context.ChatUsers.Add(new ChatUser
                {
                    Username = username,
                    IsOnline = true,
                    ConnectionId = Context.ConnectionId
                });
            }
            else
            {
                user.Username = username;
                user.IsOnline = true;
            }

            await _context.SaveChangesAsync();

            var users = _context.ChatUsers
                                .Where(u => u.IsOnline)
                                .Select(u => u.Username)
                                .ToList();

            await Clients.All.SendAsync("UpdateUserList", users);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                user.IsOnline = false;
                await _context.SaveChangesAsync();
            }

            var users = _context.ChatUsers
                                .Where(u => u.IsOnline)
                                .Select(u => u.Username)
                                .ToList();

            await Clients.All.SendAsync("UpdateUserList", users);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
