using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
            var lastDayDividendsStockNameList = lastDayDividends.Select(d => d.StockName).ToList();

            IEnumerable<DividendAlertData.Model.User> users = await _userRepository.GetAllAsync();

            Parallel.ForEach(users, (user) =>
            {
                List<Dividend> dividendsToPush = new List<Dividend>();
                string[] userStocks = user.StockList.Split(";");
                foreach (string userStock in userStocks)
                {
                    if (lastDayDividendsStockNameList.Contains(userStock))
                    {
                        dividendsToPush.Add(lastDayDividends.First(d => d.StockName == userStock));
                    }
                }

                // TODO send push to user
                string msgPush = "New dividends for " + dividendsToPush.Select(d => d.StockName);
            });




            return Ok();
        }

    }
}