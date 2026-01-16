using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GymFlow_Backend_Phase4E.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // Navigation
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<ChatMessage>? SentMessages { get; set; }
        public ICollection<ChatMessage>? ReceivedMessages { get; set; }
    }
}
