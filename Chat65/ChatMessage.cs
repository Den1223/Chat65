
namespace Chat65.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}

