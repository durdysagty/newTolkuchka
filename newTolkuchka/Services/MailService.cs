using MailKit.Net.Smtp;
using Microsoft.Extensions.Localization;
using MimeKit;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class MailService : IMail
    {
        private readonly IStringLocalizer<Shared> _localizer;
        private readonly IPath _path;

        public MailService(IStringLocalizer<Shared> localizer, IPath path)
        {
            _localizer = localizer;
            _path = path;
        }
        public async Task<bool> SendPinAsync(string email, int pin)
        {
            try
            {
                var emailMessage = new MimeMessage();

                string box = "info@tolkuchka.bar";
                emailMessage.From.Add(new MailboxAddress("", box));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = _localizer["your-pin"];
                var builder = new BodyBuilder()
                {
                    HtmlBody = string.Format(File.ReadAllText(_path.GetHtmlPinBodyPath()), _path.GetLogo(), _localizer["your-pin"], pin, _localizer["enter-pin"], _localizer["recomended-pin"])
                };
                emailMessage.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                await client.AuthenticateAsync(box, Secrets.mailPas);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
