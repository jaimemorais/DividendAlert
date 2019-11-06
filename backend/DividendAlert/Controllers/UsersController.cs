using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DividendAlert.Controllers
{
    [Route("api/[controller]")]
    public class UsersController
    {

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromForm]string email, [FromForm]string pwd)
        {
            /* TODO
            */
            return null;
        }

    }
}
