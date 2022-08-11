using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace E_commerce_app.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var message = new SendGridMessage()
            {
                From = new EmailAddress("denizgokbasprf48@gmail.com","Dng-Shop"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent=htmlMessage
            };
            message.AddTo(new EmailAddress(email));
            try
            {
                return client.SendEmailAsync(message);
            }
            catch (System.Exception)
            {

                throw;
            }
            return null;
        }
        public EmailOptions Options { get; set; }
        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
           Options = emailOptions.Value;
        }
    }
}
