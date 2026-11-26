using AgroPulse.Abstractions.DTOs.Auth;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgroPulse.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result is null)
                return Unauthorized(new { message = "Email o contraseña incorrectos." });

            return Ok(result);
        }
    }
}
