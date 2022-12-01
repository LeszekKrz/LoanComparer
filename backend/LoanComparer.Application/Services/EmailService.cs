using LoanComparer.Application.Configuration;
using LoanComparer.Application.Model;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace LoanComparer.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailAddress _fromEmailAddress;
        private readonly ISendGridClient _sendGridClient;

        public EmailService(IOptions<FromEmailConfiguration> fromEmailConfigurationOptions, ISendGridClient sendGridClient)
        {
            FromEmailConfiguration fromEmailConfiguration = fromEmailConfigurationOptions.Value;
            _fromEmailAddress = new EmailAddress(fromEmailConfiguration.EmailAddress, fromEmailConfiguration.Name);
            _sendGridClient = sendGridClient;
        }

        public async Task SendEmailAsync(Email email, CancellationToken cancellationToken)
        {
            var sendGridMessage = new SendGridMessage()
            {
                From = _fromEmailAddress,
                Subject = email.Subject,
                PlainTextContent = email.PlainTextContent,
                HtmlContent = email.HtmlContent
            };
            sendGridMessage.AddTos(email.Recipients.Select(recipient => new EmailAddress(recipient)).ToList());

            await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);
        }
    }
}
