using Api.B2B.Core.Dtos;
using Api.B2B.Core.Interfaces;
using Api.B2B.Data;
using Api.B2B.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Api.B2B.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Find the user with the provided username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // Check if user exists and verify the password with bcrypt
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Error al iniciar sesión. Verifica tus credenciales."
                };
            }

            // Generate a JWT token (implementation is as before)
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Success = true,
                Message = "Login exitoso.",
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task LogoutAsync(string token)
        {
            // Get token expiry
            var expiry = GetTokenExpiryDate(token);
            if (expiry != null)
            {
                var blacklistEntry = new TokenBlacklist
                {
                    Token = token,
                    Expiry = expiry.Value
                };

                _context.TokenBlacklist.Add(blacklistEntry);
                await _context.SaveChangesAsync();
            }
        }

        private DateTime? GetTokenExpiryDate(string token)
        {
            // Parse the JWT token and extract the expiry date
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var expClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expClaim != null && long.TryParse(expClaim, out long exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }

            return null;
        }

        public async Task<bool> IsTokenBlacklisted(string token)
        {
            return await _context.TokenBlacklist.AnyAsync(t => t.Token == token && t.Expiry > DateTime.UtcNow);
        }
    }
}
