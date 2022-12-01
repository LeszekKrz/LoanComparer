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

        public async Task<IReadOnlyCollection<JobTypeDTO>> GetAllJobTypesAsync(CancellationToken cancellationToken)
        {
            return await _context.JobTypes.Select(x => new JobTypeDTO(x.Name)).ToArrayAsync(cancellationToken);
        }
    }
}
