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

        // Метод для надсилання повідомлень
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

        // Метод для додавання користувача в онлайн список
        public async Task JoinChat(string username)
        {
            // Перевірка, чи користувач вже є
            var user = _context.ChatUsers.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                _context.ChatUsers.Add(new ChatUser { Username = username, IsOnline = true });
            }
            else
            {
                user.IsOnline = true;
            }
            await _context.SaveChangesAsync();

            // Оновлюємо список онлайн користувачів для всіх
            var users = _context.ChatUsers
                                .Where(u => u.IsOnline)
                                .Select(u => u.Username)
                                .ToList();

            await Clients.All.SendAsync("UpdateUserList", users);
        }

        // Відстеження відключення користувача
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Визначаємо користувача за Context.ConnectionId або ім'ям
            // Тут можна використати ім'я користувача, якщо зберегли його при JoinChat
            var user = _context.ChatUsers.FirstOrDefault(u => u.IsOnline && u.Username == Context.ConnectionId);
            if (user != null)
            {
                user.IsOnline = false;
                await _context.SaveChangesAsync();
            }

            // Оновлюємо список онлайн користувачів для всіх
            var users = _context.ChatUsers
                                .Where(u => u.IsOnline)
                                .Select(u => u.Username)
                                .ToList();

            await Clients.All.SendAsync("UpdateUserList", users);

            await base.OnDisconnectedAsync(exception);
        }
    }
}