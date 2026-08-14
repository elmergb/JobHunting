using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JobHunting.Controllers
{
    [ApiController]
    [Route("job/application")]
    [Produces("application/json")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;
        public JobApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateApplicationRequest request, CancellationToken ct = default )
        {
            var result = await _service.CreateAsync(request, ct);

            return result.ToActionResult(this);
        }
    }
}
