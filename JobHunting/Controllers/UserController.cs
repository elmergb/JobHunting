using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobHunting.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken ct = default)
        {
            var result = await _service.CreateAsync(request, ct);
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return result.ToActionResult(this);
        }
    }
}
