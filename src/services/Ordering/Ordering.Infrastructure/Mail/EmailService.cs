using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailService> _log { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> log)
        {
            _emailSettings = emailSettings.Value;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);

            var emailBody = email.Body;
            var subject = email.Subject;
            var to = new EmailAddress(email.To);

            var from = new EmailAddress
            {
                Email = _emailSettings.FromAddress,
                Name = _emailSettings.FromName
            };

            var msg = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(msg);

            _log.LogInformation("Email sent.");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            _log.LogError("Email sending failed");
            return false;
        }
    }
}
