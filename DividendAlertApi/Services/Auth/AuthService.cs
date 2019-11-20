using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DividendAlert.Services.Auth
{
    public class AuthService : IAuthService
    {


        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecret = Encoding.ASCII.GetBytes(_config["JwtSecret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),

                Expires = DateTime.UtcNow.AddSeconds(double.Parse(_config["JwtExpirationInSeconds"])),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }



        public string GenerateResetCode()
        {
            Random random = new Random();
            string resetCode = new string(
                Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return resetCode;
        }

        private const int ITERATIONS = 50000;

        public string GeneratePwdHash(string pwd)
        {
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pwd, salt, ITERATIONS);
            byte[] pBKDF2Bytes = pbkdf2.GetBytes(20);

            byte[] finalHash = new byte[36];
            Array.Copy(salt, 0, finalHash, 0, 16);
            Array.Copy(pBKDF2Bytes, 0, finalHash, 16, 20);

            return Convert.ToBase64String(finalHash);
        }

        public bool CheckPwd(string informedPwd, string userPwd)
        {

            byte[] userPwdBytes = Convert.FromBase64String(userPwd);
            byte[] salt = new byte[16];
            Array.Copy(userPwdBytes, 0, salt, 0, 16);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(informedPwd, salt, ITERATIONS);
            byte[] hashPwdSaltPBKDF2 = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (userPwdBytes[i + 16] != hashPwdSaltPBKDF2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
