namespace Notification.Customer.EmailService.Senders.Impl
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using Microsoft.Extensions.Logging;
    using Notification.Customer.EmailService.Models;
    using Notification.Customer.EmailService.Models.Configurations;
    using System;

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;
        private readonly EmailConfig emailConfig;

        public EmailSender(ILogger<EmailSender> logger, EmailConfig emailConfig)
        {
            this.logger = logger;
            this.emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Admin", this.emailConfig.From));

            message.To.ForEach(x => emailMessage.To.Add(new MailboxAddress("", x.Address)));

            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = string.Format("<h2 style='color:red'>{0}</h2>", message.Content)
            };

            if (message.Attachments != null &&
                message.Attachments.Any())
            {
                for (var i = 0; i < message.Attachments.Count; i++)
                {
                    bodyBuilder.Attachments.Add($"attachment{0}.jpg", message.Attachments[i]);
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage emailMessage)
        {
            using var smtpClient = new SmtpClient();

            try
            {
                smtpClient.Connect(this.emailConfig.SmtpServer);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtpClient.AuthenticateAsync(this.emailConfig.UserName, this.emailConfig.Password);

                await smtpClient.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unable to send email");
                throw;
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}