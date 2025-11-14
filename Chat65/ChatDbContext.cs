using Chat65.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat65.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        // Додати таблицю для користувачів
        public DbSet<ChatUser> ChatUsers { get; set; }
    }
}
