using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace DividendAlert.Mail
{
    public class MailSender : IMailSender
    {
        private readonly IConfiguration _config;

        public MailSender(IConfiguration config)
        {
            _config = config;
        }

        public void SendMail(string html)
        {
            return;

            var toEmail = "patcaravelli@gmail.com";
            var subject = "New Dividend Alert";
            var body = html;

            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(toEmail));
            mailMsg.From = new MailAddress("alert@dividendalert.com", "DividendAlert");
            mailMsg.Subject = subject;

            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient =
                new SmtpClient(_config["smtpAddress"], int.Parse(_config["smtpPort"]));

            smtpClient.Credentials =
                new System.Net.NetworkCredential(_config["email"], _config["emailPwd"]);

            smtpClient.Send(mailMsg);
        }

    }
}
