using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Models;
using GymFlow_Backend_Phase4E.Data;

namespace GymFlow_Backend_Phase4E.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GymFlowDbContext _db;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, GymFlowDbContext db, TokenService tokenService)
        {
            _userManager = userManager;
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<(bool Success, string Message)> RegisterUser(string email, string password, string fullName)
        {
            var exists = await _userManager.FindByEmailAsync(email);
            if (exists != null) return (false, "User already exists");

            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Client");
            return (true, "User created successfully");
        }

        public async Task<(ApplicationUser? User, string Error)> ValidateLogin(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (null, "Invalid credentials");

            if (!await _userManager.CheckPasswordAsync(user, password))
                return (null, "Invalid credentials");

            return (user, "");
        }

        public async Task<string> CreateRefreshToken(string userId)
        {
            var token = _tokenService.GenerateRefreshToken();
            var rt = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            return token;
        }

        public async Task<RefreshToken?> ValidateRefreshToken(string token)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            if (rt == null || rt.IsRevoked || rt.ExpiryDate < DateTime.UtcNow)
                return null;

            return rt;
        }

        public async Task RevokeRefreshToken(RefreshToken token)
        {
            token.IsRevoked = true;
            await _db.SaveChangesAsync();
        }
    }
}
