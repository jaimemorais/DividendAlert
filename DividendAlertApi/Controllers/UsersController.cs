using DividendAlert.Services.Auth;
using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IMailSender _mailSender;

        public UsersController(IAuthService authService, IUserRepository userRepository, IMailSender mailSender)
        {
            _authService = authService;
            _mailSender = mailSender;
            _userRepository = userRepository;
        }


        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm]string email, [FromForm]string pwd)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(pwd))
            {
                return BadRequest("Email and Password are required.");
            }

            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                return BadRequest("There's already and account with this email.");
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Password = _authService.GeneratePwdHash(pwd)
            };

            await _userRepository.InsertAsync(user);

            return Ok();
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]string email, [FromForm]string pwd)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(pwd))
            {
                return BadRequest("Email and Password are required.");
            }

            User user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !_authService.CheckPwd(pwd, user.Password))
            {
                return Unauthorized();
            }

            user.JwtToken = _authService.GenerateJwtToken(user);

            user.Password = null;

            return Ok(user);
        }


        [AllowAnonymous]
        [HttpPost("sendPasswordResetCode")]
        public async Task<IActionResult> SendResetCode([FromForm]string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            User user = await _userRepository.GetByEmailAsync(email);
            user.PasswordResetCode = _authService.GenerateResetCode();
            await _userRepository.ReplaceAsync(user);

            _mailSender.SendMail(email, "DividendAlert - Password reset", "Your password reset code : " + user.PasswordResetCode);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm]string email, [FromForm]string resetCode, [FromForm]string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(resetCode) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Email/password and reset code are required.");
            }

            User user = await _userRepository.GetByEmailAsync(email);

            if (user.PasswordResetCode == resetCode)
            {
                user.Password = _authService.GeneratePwdHash(newPassword);
                await _userRepository.ReplaceAsync(user);
            }

            return Ok();
        }
    }
}
