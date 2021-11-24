using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RecipeManagementBE.Mail;

namespace RecipeManagementBE.Service.Impl {
    public class MailService : IMailService {

        private readonly MailMetadata _mailMetadata;

        public MailService(IOptions<MailMetadata> mailMetadata) {
            _mailMetadata = mailMetadata.Value;
        }
        
        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage emailMessage)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(emailMessage.Sender);
            mimeMessage.To.Add(emailMessage.Reciever);
            mimeMessage.Subject = emailMessage.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                { Text = emailMessage.Content };
            return mimeMessage;
        }

        public void SendMail(string email, string subject, string content) {
            var message = new EmailMessage();
            message.Sender = new MailboxAddress("StaffMate", _mailMetadata.Sender);
            message.Reciever = new MailboxAddress("Account", email);
            message.Subject = subject;
            message.Content = content;
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_mailMetadata.SmtpServer,
                    _mailMetadata.Port, SecureSocketOptions.StartTls);
                smtpClient.Authenticate(_mailMetadata.UserName,
                    _mailMetadata.Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
        }
    }
}