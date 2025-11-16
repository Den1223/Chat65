using Chat65.Models;

namespace Chat65.Services
{
    public static class ChatMemory
    {
        private static readonly List<ChatMessage> _messages = new List<ChatMessage>();

        public static void AddMessage(string user, string text)
        {
            _messages.Add(new ChatMessage
            {
                User = user,
                Text = text,
                Time = DateTime.UtcNow
            });

            CleanupOld();
        }

        public static List<ChatMessage> GetLastMessages()
        {
            CleanupOld();
            return _messages.ToList();
        }

        // delete sms for 24 hour
        private static void CleanupOld()
        {
            DateTime limit = DateTime.UtcNow.AddDays(-1);
            _messages.RemoveAll(m => m.Time < limit);
        }
    }
}
