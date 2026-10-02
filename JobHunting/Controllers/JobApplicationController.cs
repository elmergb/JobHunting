using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Services.Interface;
using JobHunting.Domain.Primatives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace JobHunting.Controllers
{
    [ApiController]
    [Route("api/job/application")]
    [Produces("application/json")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;
        public JobApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateJobApplicationRequest request, CancellationToken ct = default)
        {
            var result = await _service.CreateAsync(request, ct);

            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);

            return result.ToActionResult(this);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct = default)
        {
            var result = await _service.GetUserPipelineAsync(ct);

            return result.ToActionResult(this);
        }
    }
}
