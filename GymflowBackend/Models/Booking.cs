namespace GymFlow_Backend_Phase4E.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = null!;
        public int TrainerId { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? Client { get; set; }
        public Trainer? Trainer { get; set; }
    }
}
