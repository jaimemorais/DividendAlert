namespace DividendAlert.Mail
{
    public interface IMailSender
    {
        void SendMail(string html);
    }
}
