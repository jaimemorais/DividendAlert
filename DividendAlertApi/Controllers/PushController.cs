using DividendAlertApi.Services.Push;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DividendAlertApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {

        private readonly IDividendRepository _dividendRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPushService _pushService;
        private readonly IConfiguration _config;

        public PushController(IDividendRepository dividendRepository, IUserRepository userRepository, IPushService pushService, IConfiguration config)
        {
            _dividendRepository = dividendRepository;
            _userRepository = userRepository;
            _pushService = pushService;
            _config = config;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("sendPush/{sendPushToken}")]
        public async Task<IActionResult> SendPush(string sendPushToken)
        {
            if (!_config["SendPushToken"].Equals(sendPushToken))
            {
                return Unauthorized();
            }

            IEnumerable<Dividend> lastDayDividends = await _dividendRepository.GetLastDaysDividends(1);
            var lastDayDividendsStockNameList = lastDayDividends.Select(d => d.StockName).ToList();

            IEnumerable<DividendAlertData.Model.User> users = await _userRepository.GetAllAsync();

            Parallel.ForEach(users, async (user) =>
            {
                List<Dividend> dividendsToPush = new List<Dividend>();
                string[] userStocks = user.StockList.Split(";");
                dividendsToPush.AddRange(from string userStock in userStocks
                                         where lastDayDividendsStockNameList.Contains(userStock)
                                         select lastDayDividends.First(d => d.StockName == userStock));

                string pushBody = "New dividends for " + dividendsToPush.Select(d => d.StockName);
                await _pushService.SendPushAsync(user.FirebaseCloudMessagingToken, "New Dividend!", pushBody);
            });


            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> UpdateUserFcmTokenAsync(string userFcmToken)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            user.FirebaseCloudMessagingToken = userFcmToken;

            await _userRepository.ReplaceAsync(user);

            return Ok();
        }


    }
}