using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GymFlow_Backend_Phase4E.Services;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly TokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            AuthService auth,
            TokenService tokenService,
            UserManager<ApplicationUser> userManager)
        {
            _auth = auth;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var result = await _auth.RegisterUser(
                model.Email,
                model.Password,
                model.FullName
            );

            if (!result.Success)
                return BadRequest(new { error = result.Message });

            return Ok(new { message = result.Message });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var login = await _auth.ValidateLogin(model.Email, model.Password);
            if (login.User == null)
                return Unauthorized(login.Error);

            var roles = await _userManager.GetRolesAsync(login.User);

            // Fixed: Added roles parameter and null-coalescing for email
            var accessToken = _tokenService.CreateAccessToken(
                login.User.Id,
                login.User.Email ?? string.Empty,
                roles
            );

            var refreshToken = await _auth.CreateRefreshToken(login.User.Id);

            return Ok(new
            {
                accessToken,
                refreshToken
            });
        }

        // ================= REFRESH =================
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var valid = await _auth.ValidateRefreshToken(refreshToken);
            if (valid == null) return Unauthorized("Invalid refresh token");

            var user = await _userManager.FindByIdAsync(valid.UserId);
            if (user == null) return Unauthorized("User not found");

            await _auth.RevokeRefreshToken(valid);
            var newRefresh = await _auth.CreateRefreshToken(user.Id);

            var roles = await _userManager.GetRolesAsync(user);

            // Fixed: Added roles parameter and null-coalescing for email
            var accessToken = _tokenService.CreateAccessToken(
                user.Id,
                user.Email ?? string.Empty,
                roles
            );

            return Ok(new
            {
                accessToken,
                refreshToken = newRefresh
            });
        }

        // ================= LOGOUT =================
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var valid = await _auth.ValidateRefreshToken(refreshToken);
            if (valid != null)
                await _auth.RevokeRefreshToken(valid);

            return Ok("Logged out");
        }

        // ================= GET USER BY ID =================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email
            });
        }
    }
}