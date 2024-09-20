using Notification.Customer.EmailService.Models;

namespace Notification.Customer.EmailService.Senders
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}