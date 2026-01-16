using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Data;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Services
{
    public class AvailabilityService
    {
        private readonly GymFlowDbContext _db;

        public AvailabilityService(GymFlowDbContext db)
        {
            _db = db;
        }

        // ============================
        // GET AVAILABILITY (by trainer)
        // ============================
        public async Task<List<InstructorAvailability>> GetAvailability(int trainerId)
        {
            return await _db.InstructorAvailabilities
                .Where(a => a.TrainerId == trainerId)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        // ============================
        // ADD AVAILABILITY
        // ============================
        public async Task<InstructorAvailability?> AddAvailability(
            int trainerId,
            DayOfWeek day,
            TimeSpan start,
            TimeSpan end)
        {
            if (end <= start)
                return null;

            var overlaps = await _db.InstructorAvailabilities.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.DayOfWeek == day &&
                a.StartTime < end &&
                a.EndTime > start
            );

            if (overlaps)
                return null;

            var availability = new InstructorAvailability
            {
                TrainerId = trainerId,
                DayOfWeek = day,
                StartTime = start,
                EndTime = end
            };

            _db.InstructorAvailabilities.Add(availability);
            await _db.SaveChangesAsync();

            return availability;
        }

        // ============================
        // DELETE AVAILABILITY
        // ============================
        public async Task<bool> DeleteAvailability(int availabilityId, int trainerId)
        {
            var slot = await _db.InstructorAvailabilities
                .FirstOrDefaultAsync(a =>
                    a.Id == availabilityId &&
                    a.TrainerId == trainerId);

            if (slot == null)
                return false;

            _db.InstructorAvailabilities.Remove(slot);
            await _db.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // GENERATE AVAILABLE BOOKING SLOTS (BLOCKS BOOKINGS)
        // ==================================================
        public async Task<List<BookingSlotDto>> GetAvailableBookingSlots(
            int trainerId,
            DateTime date,
            TimeSpan slotLength)
        {
            var dayOfWeek = date.DayOfWeek;

            var availability = await _db.InstructorAvailabilities
                .Where(a =>
                    a.TrainerId == trainerId &&
                    a.DayOfWeek == dayOfWeek)
                .ToListAsync();

            var bookings = await _db.Bookings
                .Where(b =>
                    b.TrainerId == trainerId &&
                    b.StartDateTime.Date == date.Date)
                .ToListAsync();

            var slots = new List<BookingSlotDto>();

            foreach (var a in availability)
            {
                var current = a.StartTime;

                while (current + slotLength <= a.EndTime)
                {
                    var slotStart = date.Date + current;
                    var slotEnd = slotStart + slotLength;

                    var isBooked = bookings.Any(b =>
                        b.StartDateTime < slotEnd &&
                        b.EndDateTime > slotStart
                    );

                    if (!isBooked)
                    {
                        slots.Add(new BookingSlotDto
                        {
                            DayOfWeek = dayOfWeek,
                            StartTime = current,
                            EndTime = current + slotLength
                        });
                    }

                    current += slotLength;
                }
            }

            return slots;
        }
    }

    // ============================
    // BOOKING SLOT DTO
    // ============================
    public class BookingSlotDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
