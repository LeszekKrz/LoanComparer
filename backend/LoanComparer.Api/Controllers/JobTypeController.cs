using LoanComparer.Application.DTO;
using LoanComparer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class JobTypeController : ControllerBase
    {
        private readonly JobTypeService _jobTypeService;

        public JobTypeController(JobTypeService jobTypeService)
        {
            _jobTypeService = jobTypeService;
        }

        [HttpGet("job-types")]
        public async Task<IReadOnlyCollection<JobTypeDTO>> GetAllJobTypes(CancellationToken cancellationToken)
            => await _jobTypeService.GetAllJobTypes(cancellationToken);
    }
}
