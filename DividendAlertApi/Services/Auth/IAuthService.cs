using DividendAlertData.Model;

namespace DividendAlert.Services.Auth
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);

        string GeneratePwdHash(string pwd);

        string GenerateResetCode();

        bool CheckPwd(string informedPwd, string userPwd);

    }
}
