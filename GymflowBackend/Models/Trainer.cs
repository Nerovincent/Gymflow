namespace GymFlow_Backend_Phase4E.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string UserId { get; set; } = "";
        public string Specialty { get; set; } = "";
        public string Bio { get; set; } = "";

        // Navigation
        public ApplicationUser? User { get; set; }

        public ICollection<Booking>? Bookings { get; set; }

        // ✅ ADD THIS
        public ICollection<InstructorAvailability> Availabilities { get; set; }
            = new List<InstructorAvailability>();
    }
}
