using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Data;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Services
{
    public class ChatService
    {
        private readonly GymFlowDbContext _db;

        public ChatService(GymFlowDbContext db)
        {
            _db = db;
        }

        public async Task<ChatMessage> SendMessage(string senderId, string receiverId, string? message, string? imageUrl)
        {
            var msg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                ImageUrl = imageUrl,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.ChatMessages.Add(msg);
            await _db.SaveChangesAsync();

            return msg;
        }

        public async Task<List<ChatMessage>> GetConversation(string user1, string user2)
        {
            return await _db.ChatMessages
                .Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                            (m.SenderId == user2 && m.ReceiverId == user1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount(string userId)
        {
            return await _db.ChatMessages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetUnreadCountsByUser(string userId)
        {
            var unreadMessages = await _db.ChatMessages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToListAsync();

            return unreadMessages.ToDictionary(x => x.SenderId, x => x.Count);
        }

        public async Task MarkAsRead(string userId, string otherUserId)
        {
            var messages = await _db.ChatMessages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _db.SaveChangesAsync();
        }
    }
}