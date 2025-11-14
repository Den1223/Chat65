using System.ComponentModel.DataAnnotations;
namespace Chat65.Models
{
    public class ChatUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}