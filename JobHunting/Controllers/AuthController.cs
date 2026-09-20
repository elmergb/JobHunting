using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobHunting.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
    
        public AuthController(IAuthService service)
        {
            _service = service;
        
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct = default)
        {
            var result = await _service.LoginAsync(request, ct);
            return result.ToActionResult(this);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe(CancellationToken ct = default)
        {
            var result = await _service.GetCurrentUser(ct);
            return result.ToActionResult(this);
        }

    }
}
