namespace GymFlow_Backend_Phase4E.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = "";
        public string ReceiverId { get; set; } = "";
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } = false;

        // Navigation
        public ApplicationUser? Sender { get; set; }
        public ApplicationUser? Receiver { get; set; }
    }
}