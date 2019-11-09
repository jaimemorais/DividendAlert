using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.Services;
using DividendAlertData.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    // TODO [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DividendsController : ControllerBase
    {
        private readonly IMailSender _mailSender;
        private readonly IDividendsHtmlBuilder _dividendsHtmlBuilder;
        private readonly IDividendListBuilder _dividendListBuilder;

        public DividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
        }



        [HttpGet]
        [Route("html")]
        [Produces("text/html")]
        public async Task<string> GetHtmlAsync(string customStockList = null)
        {
            // TODO remove
            User currentUser = new User();
            string[] stockList = currentUser.GetUserStockList();
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
            //"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            //"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";

            // TODO create a hosted service to scrape all stocks listed on the database
            // Change here to read the db

            const string uri = "https://statusinvest.com.br/acoes/";

            return await _dividendListBuilder.ScrapeAndBuildDividendListAsync(uri, "ccro3");
        }


    }
}
