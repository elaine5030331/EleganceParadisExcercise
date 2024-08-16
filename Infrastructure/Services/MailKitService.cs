using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

namespace Infrastructure.Services
{
    public class MailKitService : IEmailSender
    {
        private readonly ILogger<MailKitService> _logger;
        private readonly MailServerOptions _settings;

        public MailKitService(ILogger<MailKitService> logger, IConfiguration configuration, IOptions<MailServerOptions> options)
        {
            _logger = logger;
            _settings = options.Value;
            //_settings = new MailServerSettings()
            //{
            //    Host = configuration["MailSererSettings:Host"]!,
            //    Port = int.Parse(configuration["MailSererSettings:Port"]!),
            //    IsSSL = bool.Parse(configuration["MailSererSettings:IsSSL"]!),
            //    UserName = configuration["MailSererSettings:UserName"]!,
            //    Password = configuration["MailSererSettings:Password"]!
            //};
        }

        public async Task SendAsync(EmailDTO emailDTO)
        {
            MimeMessage message = EmailMessageHandler(emailDTO);

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_settings.Host, _settings.Port, _settings.IsSSL);
                    await client.AuthenticateAsync(_settings.UserName, _settings.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private static MimeMessage EmailMessageHandler(EmailDTO emailDTO)
        {
            var message = new MimeMessage();

            if (string.IsNullOrEmpty(emailDTO.MailFrom))
                emailDTO.MailFrom = "EleganceParadis Servie";

            if (string.IsNullOrEmpty(emailDTO.MailFromEmail))
                emailDTO.MailFromEmail = "eleganceparadis@gmail.com";

            if(string.IsNullOrEmpty(emailDTO.MailToEmail))
                throw new ArgumentNullException(nameof(emailDTO.MailToEmail), "收件人信箱為必填");

            message.From.Add(new MailboxAddress(Encoding.UTF8, emailDTO.MailFrom, emailDTO.MailFromEmail));
            message.To.Add(new MailboxAddress(Encoding.UTF8, emailDTO.MailTo, emailDTO.MailToEmail));
            message.Subject = emailDTO.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = emailDTO.HTMLContent;
            message.Body = builder.ToMessageBody();

            return message;
        }
    }
}
