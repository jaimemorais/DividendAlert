using DividendAlert.Services.Auth;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
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
                Id = new System.Guid(),
                Email = email,
                Password = pwd
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

    }
}
