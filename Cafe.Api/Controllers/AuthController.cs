using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService): ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
        {
            var result = await authService.RegisterAsync(request, ct);
            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        {
            var result = await authService.LoginAsync(request, ct);
            return Ok(result);
        }
    }
}
    