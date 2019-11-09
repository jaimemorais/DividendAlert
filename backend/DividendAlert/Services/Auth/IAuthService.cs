using DividendAlertData.Model;

namespace DividendAlert.Services.Auth
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
    }
}
