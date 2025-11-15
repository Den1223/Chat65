using Chat65.Data;
using Chat65.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chat65.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        
        public async Task JoinChat(string username)
        {
            var connectionId = Context.ConnectionId;

            var existing = await _context.ChatUsers
                .FirstOrDefaultAsync(u => u.ConnectionId == connectionId);

            if (existing == null)
            {
                _context.ChatUsers.Add(new ChatUser
                {
                    Username = username,
                    ConnectionId = connectionId,
                    IsOnline = true
                });
            }
            else
            {
                existing.Username = username;
                existing.IsOnline = true;
            }

            await _context.SaveChangesAsync();

            
            await UpdateUserList();

            
            await LoadRecentMessages();
        }

        
        public async Task SendMessage(string user, string message)
        {
            var msg = new Message
            {
                User = user,
                Text = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        
        public async Task LoadRecentMessages()
        {
            DateTime since = DateTime.UtcNow.AddDays(-1);

            var messages = await _context.Messages
                .Where(m => m.Timestamp >= since)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        
        private async Task UpdateUserList()
        {
            var users = await _context.ChatUsers
                .Where(u => u.IsOnline)
                .Select(u => u.Username)
                .ToListAsync();

            await Clients.All.SendAsync("UpdateUserList", users);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _context.ChatUsers
                .FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                user.IsOnline = false;
                await _context.SaveChangesAsync();
                await UpdateUserList();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
