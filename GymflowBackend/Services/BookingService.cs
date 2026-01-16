using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Data;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Services
{
    public class BookingService
    {
        private readonly GymFlowDbContext _db;

        public BookingService(GymFlowDbContext db)
        {
            _db = db;
        }

        // ============================
        // CREATE BOOKING (SLOT-BASED)
        // ============================
        public async Task<Booking?> CreateBooking(
            string clientId,
            string trainerUserId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var start = date.Date + startTime;
            var end = date.Date + endTime;

            if (end <= start)
                return null;

            // Find the trainer by UserId to get the TrainerId
            var trainer = await _db.Trainers
                .FirstOrDefaultAsync(t => t.UserId == trainerUserId);

            if (trainer == null)
                return null; // Trainer not found

            // Prevent double booking
            var exists = await _db.Bookings.AnyAsync(b =>
                b.TrainerId == trainer.Id &&
                b.StartDateTime < end &&
                b.EndDateTime > start
            );

            if (exists)
                return null;

            var booking = new Booking
            {
                ClientId = clientId,
                TrainerId = trainer.Id,
                StartDateTime = start,
                EndDateTime = end
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return booking;
        }

        // ============================
        // CLIENT → BOOKINGS
        // ============================
        public async Task<List<Booking>> GetClientBookings(string clientId)
        {
            return await _db.Bookings
                .Where(b => b.ClientId == clientId)
                .Include(b => b.Trainer!)
                    .ThenInclude(t => t.User!)
                .OrderBy(b => b.StartDateTime)
                .ToListAsync();
        }

        // ============================
        // TRAINER (ID) → BOOKINGS
        // ============================
        public async Task<List<Booking>> GetTrainerBookings(int trainerId)
        {
            return await _db.Bookings
                .Where(b => b.TrainerId == trainerId)
                .Include(b => b.Client!)
                .OrderBy(b => b.StartDateTime)
                .ToListAsync();
        }

        // ============================
        // INSTRUCTOR → CLIENTS
        // ============================
        public async Task<List<object>> GetClientsForInstructor(string instructorUserId)
        {
            return await _db.Bookings
                .Include(b => b.Client)
                .Include(b => b.Trainer)
                .Where(b => b.Trainer!.UserId == instructorUserId)
                .Select(b => new
                {
                    b.Client!.Id,
                    b.Client.FullName,
                    b.Client.Email
                })
                .Distinct()
                .ToListAsync<object>();
        }

        // ============================
        // INSTRUCTOR → BOOKINGS
        // ============================
        public async Task<List<object>> GetBookingsForInstructor(string instructorUserId)
        {
            return await _db.Bookings
                .Include(b => b.Client)
                .Include(b => b.Trainer)
                .Where(b => b.Trainer!.UserId == instructorUserId)
                .OrderBy(b => b.StartDateTime)
                .Select(b => new
                {
                    b.Id,
                    ClientName = b.Client!.FullName,
                    ClientEmail = b.Client.Email,
                    b.StartDateTime,
                    b.EndDateTime
                })
                .ToListAsync<object>();
        }

        // ============================
        // INSTRUCTOR → CANCEL BOOKING
        // ============================
        public async Task<bool> CancelBookingByInstructor(
            int bookingId,
            string instructorUserId)
        {
            var booking = await _db.Bookings
                .Include(b => b.Trainer)
                .FirstOrDefaultAsync(b =>
                    b.Id == bookingId &&
                    b.Trainer!.UserId == instructorUserId);

            if (booking == null)
                return false;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingByClient(int bookingId, string clientId)
        {
            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b =>
                    b.Id == bookingId &&
                    b.ClientId == clientId);

            if (booking == null)
                return false;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}