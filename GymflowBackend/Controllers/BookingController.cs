using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Services;
using GymFlow_Backend_Phase4E.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace GymFlow_Backend_Phase4E.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingController(BookingService service)
        {
            _service = service;
        }

        // ============================
        // CREATE BOOKING (SLOT-BASED)
        // ============================
        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
                return Unauthorized();

            var booking = await _service.CreateBooking(
                clientId,
                request.TrainerUserId,
                request.Date,
                request.StartTime,
                request.EndTime
            );

            if (booking == null)
                return BadRequest(new { message = "Unable to create booking. Time slot may be unavailable or invalid." });

            // ✅ Return only the necessary data without circular references
            return Ok(new
            {
                id = booking.Id,
                clientId = booking.ClientId,
                trainerId = booking.TrainerId,
                startDateTime = booking.StartDateTime,
                endDateTime = booking.EndDateTime,
                message = "Booking created successfully"
            });
        }

        // ============================
        // CLIENT → BOOKINGS
        // ============================
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientBookings(string clientId)
        {
            return Ok(await _service.GetClientBookings(clientId));
        }

        // ============================
        // TRAINER → BOOKINGS
        // ============================
        [HttpGet("trainer/{trainerId}")]
        public async Task<IActionResult> GetTrainerBookings(int trainerId)
        {
            return Ok(await _service.GetTrainerBookings(trainerId));
        }

        // ============================
        // INSTRUCTOR → CANCEL BOOKING
        // ============================
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(
            int bookingId,
            [FromQuery] string instructorUserId)
        {
            var success = await _service
                .CancelBookingByInstructor(bookingId, instructorUserId);

            if (!success)
                return NotFound();

            return Ok("Booking cancelled");
        }

        // ============================
        // CLIENT → CANCEL OWN BOOKING
        // ============================
        [Authorize(Roles = "Client")]
        [HttpDelete("my-booking/{bookingId}")]
        public async Task<IActionResult> CancelMyBooking(int bookingId)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
                return Unauthorized();

            var success = await _service.CancelBookingByClient(bookingId, clientId);

            if (!success)
                return NotFound(new { message = "Booking not found or already cancelled." });

            return Ok(new { message = "Booking cancelled successfully" });
        }
    }
}
