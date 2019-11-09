using DividendAlertData.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DividendAlert.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromForm]string email, [FromForm]string pwd)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(pwd))
            {
                return BadRequest("Email and Password are required.");
            }


            // TODO get user
            // TODO create db
            User user = null;

            if (user == null)
            {
                return Unauthorized("Invalid Email/Password.");
            }



            user.JwtToken = null; // TODO _authService.GenerateJwtToken(user);

            // Nao retornar a senha no json
            user.Password = null;

            return Ok(user);
        }

    }
}
