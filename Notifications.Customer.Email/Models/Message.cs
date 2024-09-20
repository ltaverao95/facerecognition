namespace Notification.Customer.EmailService.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public class Message
    {
        public Message(IEnumerable<string> to,
                       string subject,
                       string content,
                       List<byte[]> attachments)
        {
            To = [..to.Select(x => new MailAddress(x))];
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }

        public List<MailAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<byte[]> Attachments { get; set; }
    }
}