using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using DividendAlertData.Util;
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
        private readonly IMailSender _mailSender;
        private readonly IDividendsHtmlBuilder _dividendsHtmlBuilder;
        private readonly IDividendListBuilder _dividendListBuilder;
        private readonly IUserRepository _userRepository;
        private readonly IDividendRepository _dividendRepository;

        public DividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder,
            IUserRepository userRepository, IDividendRepository dividendRepository)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
            _userRepository = userRepository;
            _dividendRepository = dividendRepository;
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
            // TODO create a hosted service to scrape all stocks listed on the database and remove the return below
            ////"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            ////"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";            
            return await _dividendListBuilder.ScrapeAndBuildDividendListAsync("https://statusinvest.com.br/acoes/", "ccro3");



            var claims = HttpContext.User.Identity as ClaimsIdentity;
            User user = await _userRepository.GetByEmailAsync(claims.FindFirst("Email").Value);

            ////stocks = "ITSA;BBSE;CCRO;RADL;ABEV;EGIE;HGTX;WEGE;FLRY"
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


    }
}
