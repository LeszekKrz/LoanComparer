using LoanComparer.Application.DTO;

namespace LoanComparer.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public IEnumerable<ErrorResponseDTO> Errors { get; }

        public BadRequestException(IEnumerable<ErrorResponseDTO> errors)
        {
            this.Errors = errors;
        }
    }
}
