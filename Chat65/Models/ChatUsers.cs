using System.ComponentModel.DataAnnotations;

namespace Chat65.Models
{
    public class ChatUser
    {
        public int Id { get; set; }               
        public string Username { get; set; } 
        public string ConnectionId { get; set; }
        public bool IsOnline { get; set; }        
    }

}