namespace Chat65.Models
{
    public class Message
    {
        public int Id { get; set; }           // Первичный ключ
        public string User { get; set; } = ""; // Имя пользователя
        public string Text { get; set; } = ""; // Текст сообщения
        public DateTime Timestamp { get; set; } // Время отправки
    }
}
