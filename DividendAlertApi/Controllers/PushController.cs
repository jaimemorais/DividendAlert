using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {

        private readonly IDividendRepository _dividendRepository;
        private readonly IUserRepository _userRepository;


        public PushController(IDividendRepository dividendRepository, IUserRepository userRepository)
        {
            _dividendRepository = dividendRepository;
            _userRepository = userRepository;
        }


        [HttpPost]
        [Route("sendPush")]
        public async Task<IActionResult> SendPush()
        {
            IEnumerable<Dividend> lastDayDividends = await _dividendRepository.GetLastDaysDividends(1);
            IEnumerable<DividendAlertData.Model.User> users = await _userRepository.GetAllAsync();



            Parallel.ForEach(users, (user) =>
            {
                string[] userStocks = user.StockList.Split(";");
                //if (userStocks.Any(lastDayDividends)) 
                {
                    // TODO send push
                }

            });




            return Ok();
        }

    }
}