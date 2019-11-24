using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace DividendAlert.Services.Mail
{
    public class MailSender : IMailSender
    {
        private readonly IConfiguration _config;

        public MailSender(IConfiguration config)
        {
            _config = config;
        }

        public void SendMail(string to, string subject, string html)
        {
            var body = html;

            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(to));
            mailMsg.From = new MailAddress("alert@dividendalert.com", "DividendAlert");
            mailMsg.Subject = subject;

            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient =
                new SmtpClient(_config["dividendAlert_smtpAddress"], int.Parse(_config["dividendAlert_smtpPort"]));

            smtpClient.Credentials =
                new System.Net.NetworkCredential(_config["dividendAlert_email"], _config["dividendAlert_emailPwd"]);

            smtpClient.Send(mailMsg);
        }

    }
}
