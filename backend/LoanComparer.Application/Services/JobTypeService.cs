using LoanComparer.Application.DTO;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application.Services
{
    public class JobTypeService
    {
        private readonly LoanComparerContext _context;

        public JobTypeService(LoanComparerContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<JobTypeDTO>> GetAllJobTypes(CancellationToken cancellationToken)
        {
            Thread.Sleep(10000);
            return await _context.JobTypes.Select(x => new JobTypeDTO(x.Name)).ToArrayAsync(cancellationToken);

        }
    }
}
