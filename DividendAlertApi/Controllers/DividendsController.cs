using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DividendsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDividendRepository _dividendRepository;


        public DividendsController(IUserRepository userRepository, IDividendRepository dividendRepository)
        {
            _userRepository = userRepository;
            _dividendRepository = dividendRepository;
        }


        [HttpGet]
        [Route("lastDays/{days}")]
        [Produces("application/json")]
        public async Task<IEnumerable<Dividend>> GetLastDividends(int days)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            string[] stockList = user.StockList.Split(";");

            List<Dividend> result = new List<Dividend>();
            foreach (string stockName in stockList)
            {
                var dividendList = await _dividendRepository.GetByStockNameAsync(stockName);
                var lastMonth = dividendList.Where(d => d.DateAdded.AddDays(-days) >= DateTime.Now);
                result.AddRange(lastMonth);
            }

            return result;
        }



    }
}
