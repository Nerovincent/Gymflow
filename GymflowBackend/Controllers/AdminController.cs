using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Constants;
using GymFlow_Backend_Phase4E.Models;
using GymFlow_Backend_Phase4E.Services;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TrainerService _trainerService;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            TrainerService trainerService)
        {
            _userManager = userManager;
            _trainerService = trainerService;
        }

        // ================= GET ALL USERS =================
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // ================= PROMOTE CLIENT → INSTRUCTOR =================
        [HttpPost("promote/{userId}")]
        public async Task<IActionResult> PromoteToInstructor(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            // ❌ Block invalid promotions
            if (roles.Contains(Roles.Admin))
                return BadRequest("Cannot modify admin role");
            if (roles.Contains(Roles.Instructor))
                return BadRequest("User is already an instructor");

            // ✅ Promote Client → Instructor
            await _userManager.AddToRoleAsync(user, Roles.Instructor);

            // 🔑 CREATE TRAINER PROFILE IF MISSING
            var trainer = await _trainerService.GetByUserId(userId);
            if (trainer == null)
            {
                await _trainerService.CreateTrainer(
                    userId,
                    specialty: "General Fitness",
                    bio: "Certified instructor"
                );
            }

            return Ok(new { message = "User promoted to Instructor" });
        }

        // ================= DELETE USER =================
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            // ❌ Prevent deleting admin accounts
            if (roles.Contains(Roles.Admin))
                return BadRequest("Cannot delete admin users");

            // 🗑️ Delete the user (Identity handles cascading deletes)
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to delete user",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new { message = $"User {user.Email} deleted successfully" });
        }
    }
}