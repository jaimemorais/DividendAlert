using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public DividendsController(IUserRepository userRepository, IDividendRepository dividendRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _dividendRepository = dividendRepository;

            _config = configuration;
        }


        [AllowAnonymous] // TODO remove
        [HttpGet]
        [Route("lastDays/{scrapeToken}/{days}")]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<Dividend>>> GetLastDaysDividends(string scrapeToken, int days)
        {
            // TODO remove
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            claims.AddClaim(new Claim("Email", "jaime@teste.com")); // TODO remove
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            if (user == null)
            {
                return NotFound();
            }

            string[] stockList = user.StockList.Split(";");

            List<Dividend> lastDaysDividendList = new List<Dividend>();

            foreach (string stockName in stockList)
            {
                var dividendList = await _dividendRepository.GetByStockNameAsync(stockName);

                lastDaysDividendList.AddRange(dividendList.Where(d => d.DateAdded >= DateTime.Now.AddDays(-days)));
            }

            return lastDaysDividendList;
        }


        [AllowAnonymous] // TODO remove
        [HttpGet]
        [Route("next/{scrapeToken}/{year}/{month}/{day}")]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<Dividend>>> GetNextDividends(string scrapeToken, int year, int month, int day)
        {
            // TODO remove
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            claims.AddClaim(new Claim("Email", "jaime@teste.com")); // TODO remove
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            if (user == null)
            {
                return NotFound();
            }
            /*TODO
            string[] stockList = user.StockList.Split(";");

            List<Dividend> lastDaysDividendList = new List<Dividend>();

            foreach (string stockName in stockList)
            {
                var dividendList = await _dividendRepository.GetByStockNameAsync(stockName);

                lastDaysDividendList.AddRange(dividendList.Where(d => d.PaymentDate >= DateTime.Now.AddDays(-days)));
            }
            
            return lastDaysDividendList;
            */
            return null;
        }
    }
}
