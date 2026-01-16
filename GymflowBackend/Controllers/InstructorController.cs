using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Services;
using GymFlow_Backend_Phase4E.Models;
using System.Security.Claims;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [Authorize(Roles = "Instructor")]
    [ApiController]
    [Route("api/instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly TrainerService _trainerService;
        private readonly IWebHostEnvironment _env;

        public InstructorController(
            BookingService bookingService,
            TrainerService trainerService,
            IWebHostEnvironment env)
        {
            _bookingService = bookingService;
            _trainerService = trainerService;
            _env = env;
        }

        // ============================
        // INSTRUCTOR → PROFILE
        // ============================
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            return Ok(trainer);
        }

        // ============================
        // INSTRUCTOR → UPLOAD PROFILE IMAGE
        // ============================
        [HttpPost("profile/image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound("Trainer profile not found");

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            trainer.ProfileImageUrl = $"/uploads/{fileName}";
            await _trainerService.UpdateTrainer(trainer);

            return Ok(new { trainer.ProfileImageUrl });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateInstructorProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
                return NotFound();

            trainer.Specialty = request.Specialty;
            trainer.Bio = request.Bio;

            if (trainer.User != null)
                trainer.User.FullName = request.FullName;

            await _trainerService.UpdateTrainer(trainer);
            return Ok();
        }

        // ============================
        // INSTRUCTOR → CLIENTS
        // ============================
        [HttpGet("clients")]
        public async Task<IActionResult> GetMyClients()
        {
            var instructorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instructorUserId))
                return Unauthorized("Instructor ID missing from token");

            var clients = await _bookingService.GetClientsForInstructor(instructorUserId);
            return Ok(clients);
        }

        // ============================
        // INSTRUCTOR → BOOKINGS
        // ============================
        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var instructorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instructorUserId))
                return Unauthorized("Instructor ID missing from token");

            var bookings = await _bookingService.GetBookingsForInstructor(instructorUserId);
            return Ok(bookings);
        }

        // ============================
        // INSTRUCTOR → CANCEL BOOKING
        // ============================
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var instructorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instructorUserId))
                return Unauthorized("Instructor ID missing from token");

            var success = await _bookingService.CancelBookingByInstructor(id, instructorUserId);
            if (!success)
                return NotFound("Booking not found or not owned by instructor");

            return Ok(new { message = "Booking cancelled" });
        }
    }
}
