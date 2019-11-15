namespace DividendAlert.Services.Mail
{
    public interface IMailSender
    {
        void SendMail(string to, string html);
    }
}
