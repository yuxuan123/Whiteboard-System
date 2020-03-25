using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace WhiteboardAPI.Helpers
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions>optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute("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", subject, message, email);
        }

        private Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@whiteboard.com", "Whiteboard Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));
            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);

        }
    }
}
