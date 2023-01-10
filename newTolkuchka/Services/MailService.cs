using MailKit.Net.Smtp;
using Microsoft.Extensions.Localization;
using MimeKit;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Net.NetworkInformation;

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
            var builder = new BodyBuilder()
            {
                HtmlBody = string.Format(File.ReadAllText(_path.GetHtmlPinBodyPath()), _path.GetLogo(), _localizer["your-pin"], pin, _localizer["enter-pin"], _localizer["recomended-pin"])
            };
            bool result = await SendMessage(email, builder, "your-pin");
            return result;
        }

        public async Task<bool> SendRecoveryAsync(string email, Guid guid)
        {
            var builder = new BodyBuilder()
            {
                HtmlBody = string.Format(File.ReadAllText(_path.GetHtmlRecoveryBodyPath()), _path.GetLogo(), _localizer["mail-reason"], $"{CultureProvider.SiteUrlRu}/recovery/newpin/{guid}", _localizer["new-pin-link"])
            };
            bool result = await SendMessage(email, builder, "password-recovery");
            return result;
        }

        public async Task<bool> SendNewPinAsync(string email, int pin)
        {
            var builder = new BodyBuilder()
            {
                HtmlBody = string.Format(File.ReadAllText(_path.GetHtmlNewPinBodyPath()), _path.GetLogo(), _localizer["your-pin"], pin, _localizer["enter-pin"], _localizer["recomended-pin"])
            };
            bool result = await SendMessage(email, builder, "your-pin");
            return result;
        }

        private async Task<bool> SendMessage(string email, BodyBuilder bodyBuilder, string subject)
        {
            try
            {
                var emailMessage = new MimeMessage();
                string box = Secrets.infoMail;
                emailMessage.From.Add(new MailboxAddress("", box));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = _localizer[subject];
                emailMessage.Body = bodyBuilder.ToMessageBody();
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                //await client.ConnectAsync("smtp.mail.ru", 587, false);
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
