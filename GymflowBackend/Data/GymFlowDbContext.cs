using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Data
{
    public class GymFlowDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymFlowDbContext(DbContextOptions<GymFlowDbContext> options)
            : base(options) { }

        // ============================
        // DB SETS
        // ============================
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<InstructorAvailability> InstructorAvailabilities => Set<InstructorAvailability>();

        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // ============================
        // MODEL CONFIGURATION
        // ============================
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ============================
            // Trainer -> User
            // ============================
            builder.Entity<Trainer>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // Booking -> Client
            // ============================
            builder.Entity<Booking>()
                .HasOne(b => b.Client)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // Booking -> Trainer
            // ============================
            builder.Entity<Booking>()
                .HasOne(b => b.Trainer)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // InstructorAvailability -> Trainer
            // ============================
         

            // ============================
            // ChatMessage -> Sender
            // ============================
            builder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // ChatMessage -> Receiver
            // ============================
            builder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
