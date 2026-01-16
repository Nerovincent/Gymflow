using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GymFlow_Backend_Phase4E.Services;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [Authorize(Roles = "Instructor")]
    [ApiController]
    [Route("api/instructor/availability")]
    public class InstructorAvailabilityController : ControllerBase
    {
        private readonly AvailabilityService _availabilityService;
        private readonly TrainerService _trainerService;

        public InstructorAvailabilityController(
            AvailabilityService availabilityService,
            TrainerService trainerService)
        {
            _availabilityService = availabilityService;
            _trainerService = trainerService;
        }

        // ============================
        // GET AVAILABILITY
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAvailability()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            var availability = await _availabilityService.GetAvailability(trainer.Id);

            var result = availability.Select(slot => new
            {
                slot.Id,
                slot.DayOfWeek,
                slot.StartTime,
                slot.EndTime
            });

            return Ok(result);
        }

        // ============================
        // ADD AVAILABILITY
        // ============================
        [HttpPost]
        public async Task<IActionResult> AddAvailability(
            [FromBody] AddAvailabilityRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            var slot = await _availabilityService.AddAvailability(
                trainer.Id,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime
            );

            if (slot == null)
                return BadRequest("Invalid or overlapping availability slot");

            return Ok(new
            {
                slot.Id,
                slot.DayOfWeek,
                slot.StartTime,
                slot.EndTime
            });
        }

        // ============================
        // DELETE AVAILABILITY
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            var success = await _availabilityService
                .DeleteAvailability(id, trainer.Id);

            if (!success)
                return NotFound("Availability slot not found");

            return Ok();
        }

        // ==================================================
        // GET AVAILABLE BOOKING SLOTS (BLOCKS BOOKED SLOTS)
        // ==================================================
        [HttpGet("booking-slots")]
        public async Task<IActionResult> GetAvailableBookingSlots(
            [FromQuery] DateTime date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            var slots = await _availabilityService.GetAvailableBookingSlots(
                trainer.Id,
                date,
                TimeSpan.FromMinutes(30)
            );

            return Ok(slots);
        }
    }

    // ============================
    // REQUEST DTO
    // ============================
    public class AddAvailabilityRequest
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
