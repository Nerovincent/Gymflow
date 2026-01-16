using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Services;
using GymFlow_Backend_Phase4E.Data;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [Authorize(Roles = "Client")]
    [ApiController]
    [Route("api/client/booking-slots")]
    public class ClientBookingSlotsController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;
        private readonly TrainerService _trainerService;
        private readonly GymFlowDbContext _db;

        public ClientBookingSlotsController(
            AvailabilityService availabilityService,
            TrainerService trainerService,
            GymFlowDbContext db)
        {
            _availabilityService = availabilityService;
            _trainerService = trainerService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] string trainerUserId,
            [FromQuery] DateTime date)
        {
            var trainer = await _trainerService.GetByUserId(trainerUserId);
            if (trainer == null)
                return NotFound("Trainer not found");

            // Generate ALL slots from availability
            var allSlots = await _availabilityService.GetAvailableBookingSlots(
                trainer.Id,
                date,
                TimeSpan.FromMinutes(60)
            );

            // Get booked times for this date
            var bookedTimes = _db.Bookings
                .Where(b => b.TrainerId == trainer.Id && b.StartDateTime.Date == date.Date)
                .Select(b => new {
                    StartTime = b.StartDateTime.TimeOfDay,
                    EndTime = b.EndDateTime.TimeOfDay
                })
                .ToList();

            // Filter OUT booked slots - only return available ones
            var availableSlots = allSlots
                .Where(slot => !bookedTimes.Any(b =>
                    b.StartTime == slot.StartTime &&
                    b.EndTime == slot.EndTime))
                .Select(slot => new
                {
                    slot.DayOfWeek,
                    slot.StartTime,
                    slot.EndTime
                })
                .ToList();

            return Ok(availableSlots);
        }
    }
}