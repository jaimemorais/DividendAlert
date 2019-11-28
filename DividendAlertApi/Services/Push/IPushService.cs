using System.Threading.Tasks;

namespace DividendAlertApi.Services.Push
{
    public interface IPushService
    {
        Task SendPushAsync(string userFcmToken, string title, string body);
    }
}
