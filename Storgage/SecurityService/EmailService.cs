namespace Weezlabs.Storgage.SecurityService
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Email service
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        /// <summary>
        /// Send message async
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>Task for sending message.</returns>
        public Task SendAsync(IdentityMessage message)
        {
            SmtpClient client = new SmtpClient();
            var mail = new MailMessage()
            {
                To = {message.Destination},
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            return client.SendMailAsync(mail);
        }
    }
}
