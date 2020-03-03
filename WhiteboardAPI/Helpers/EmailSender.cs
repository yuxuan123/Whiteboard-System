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
                From = new EmailAddress("ahmao@gmail.com", "Ah Mao"),
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
