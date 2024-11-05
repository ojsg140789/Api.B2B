using Api.B2B.Core.Dtos;
using Api.B2B.Core.Interfaces;
using Api.B2B.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Api.B2B.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly BlacklistService _blacklistService;

        public AuthController(IAuthService authService, BlacklistService blacklistService)
        {
            _authService = authService;
            _blacklistService = blacklistService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Obtener el token desde el header de autorización
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required for logout" });
            }

            // Obtener la fecha de expiración del token
            var expiry = GetTokenExpiryDate(token);
            if (expiry == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            // Añadir el token a la lista negra
            _blacklistService.BlacklistToken(token, expiry.Value);

            return Ok(new { message = "Logged out successfully" });
        }

        // Método auxiliar para obtener la fecha de expiración de un token JWT
        private DateTime? GetTokenExpiryDate(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var expClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expClaim != null && long.TryParse(expClaim, out long exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }

            return null;
        }
    }
}
