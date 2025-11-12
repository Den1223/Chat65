
using Microsoft.EntityFrameworkCore;
using Chat65.Models; // заміни на справжній namespace, де ChatMessage.cs

namespace Chat65.Data // заміни на namespace твого проекту
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        public DbSet<ChatMessage> Messages { get; set; }
    }
}
