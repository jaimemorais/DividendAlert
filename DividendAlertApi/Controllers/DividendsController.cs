using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using DividendAlertData.Util;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DividendsController : ControllerBase
    {
        private readonly IMailSender _mailSender;
        private readonly IDividendsHtmlBuilder _dividendsHtmlBuilder;
        private readonly IDividendListBuilder _dividendListBuilder;
        private readonly IUserRepository _userRepository;
        private readonly IDividendRepository _dividendRepository;
        private readonly IStockRepository _stockRepository;

        public DividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder,
            IUserRepository userRepository, IDividendRepository dividendRepository, IStockRepository stockRepository)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
            _userRepository = userRepository;
            _dividendRepository = dividendRepository;
            _stockRepository = stockRepository;
        }


        // TODO remove this temporary method (also remove HtmlOutputFormatter class)
        [AllowAnonymous]
        [HttpGet]
        [Route("html")]
        [Produces("text/html")]
        public async Task<string> GetHtmlAsync(string customStockList = null)
        {
            User currentUser = new User();
            string[] stockList = currentUser.StockList.Split(";");

            if (customStockList != null)
            {
                stockList = customStockList.Split(";");
            }

            string html = await _dividendsHtmlBuilder.GenerateHtmlAsync(stockList);

            bool hasNewDividends = !html.Contains(Constants.NO_DIVIDENDS_FOR_TODAY);

            if (hasNewDividends)
            {
                _mailSender.SendMail("jaimemorais@gmail.com", html);
            }

            return html;
        }


        [HttpGet]
        [Route("json")]
        [Produces("application/json")]
        public async Task<IEnumerable<Dividend>> GetJsonAsync()
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            //// "ITSA;BBSE;CCRO;RADL;ABEV;EGIE;HGTX;WEGE;FLRY".Split(";");
            string[] stockList = user.StockList.Split(";");


            List<Dividend> result = new List<Dividend>();
            foreach (string stockName in stockList)
            {
                var dividendList = await _dividendRepository.GetByStockNameAsync(stockName);
                var last7Days = dividendList.Where(d => d.DateAdded.AddDays(-7) >= DateTime.Now);
                result.AddRange(last7Days);
            }

            return result;
        }




        [HttpGet]
        [Route("scrape")]
        public async Task<IActionResult> Scrape()
        {
            ////"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            ////"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";            
            const string DIVIDEND_SITE_URI = "https://statusinvest.com.br/acoes/";

            /*
            await _stockRepository.InsertAsync(new Stock()
            {
                Id = new Guid(),
                Name = "CCRO3"
            });
            */

            IList<Stock> stockList = await _stockRepository.GetAllAsync();

            foreach (Stock stock in stockList)
            {
                IEnumerable<Dividend> scrapedList = await _dividendListBuilder.ScrapeAndBuildDividendListAsync(DIVIDEND_SITE_URI, stock.Name);

                foreach (Dividend scrapedDividend in scrapedList)
                {
                    if (!(await _dividendRepository.GetByStockAsync(scrapedDividend)).Any())
                    {
                        scrapedDividend.DateAdded = DateTime.Today;
                        await _dividendRepository.InsertAsync(scrapedDividend);
                    }
                }
            }

            return Ok();
        }

    }
}
