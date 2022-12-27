using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO
{
    public record GovernmentIdDTO(string Type, string Value)
    {
        public GovernmentId ToGovernmentId()
        {
            throw new NotImplementedException();
        }
    }
}
