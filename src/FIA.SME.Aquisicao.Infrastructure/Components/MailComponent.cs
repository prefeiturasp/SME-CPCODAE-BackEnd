using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace FIA.SME.Aquisicao.Infrastructure.Components
{
    public interface IMailComponent
    {
        Task SendEmail(string toAddress, string toName, string subject, string bodyHtml);
    }

    internal class MailComponent : IMailComponent
    {
        private readonly IConfiguration _configuration;

        public MailComponent(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task SendEmail(string toAddress, string toName, string subject, string bodyHtml)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(this._configuration["Email:From:Name"], this._configuration["Email:From:Address"]));
            email.To.Add(new MailboxAddress(toName, toAddress));

            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = bodyHtml };

            using (var smtp = new SmtpClient())
            {
                if (!Int32.TryParse(this._configuration["Email:Port"], out int port))
                    port = 587;

                smtp.Connect(this._configuration["Email:Host"], port, SecureSocketOptions.StartTls);
                smtp.Authenticate(this._configuration["Email:Username"], this._configuration["Email:Password"]);
                
                await smtp.SendAsync(email);
                
                smtp.Disconnect(true);
            }
        }
    }
}
