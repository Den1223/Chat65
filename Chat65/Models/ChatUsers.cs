using System.ComponentModel.DataAnnotations;

namespace Chat65.Models
{
    public class ChatUser
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = "";

        public bool IsOnline { get; set; } = false;

        // Додатково зберігаємо ConnectionId для точного відключення
        public string ConnectionId { get; set; } = "";
    }
}