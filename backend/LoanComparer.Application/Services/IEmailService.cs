using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken);
    }
}
