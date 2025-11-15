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
            var user = await _context.ChatUsers
                .FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);

            if (user == null)
            {
                _context.ChatUsers.Add(new ChatUser
                {
                    Username = username,
                    ConnectionId = Context.ConnectionId,
                    IsOnline = true
                });
            }
            else
            {
                user.Username = username;
                user.IsOnline = true;
            }

            await _context.SaveChangesAsync();

            
            await LoadHistory();

            
            await UpdateUserList();
        }

       
        public async Task LoadHistory()
        {
            DateTime since = DateTime.Now.AddDays(-1);

            var messages = await _context.Messages
                .Where(m => m.Timestamp >= since)
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    user = m.User,
                    text = m.Text,
                    timestamp = m.Timestamp
                })
                .ToListAsync();

            await Clients.Caller.SendAsync("LoadHistory", messages);
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

            
            await CleanupOldMessages();
        }

        private async Task CleanupOldMessages()
        {
            DateTime limit = DateTime.Now.AddDays(-1);

            var oldMessages = _context.Messages
                .Where(m => m.Timestamp < limit);

            _context.Messages.RemoveRange(oldMessages);
            await _context.SaveChangesAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
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


        private async Task UpdateUserList()
        {
            var users = await _context.ChatUsers
                                      .Where(u => u.IsOnline)
                                      .Select(u => u.Username)
                                      .ToListAsync();

            await Clients.All.SendAsync("UpdateUserList", users);
        }
    }
}
