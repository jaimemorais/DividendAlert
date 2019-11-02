using DividendAlert.Mail;
using DividendAlertData.Model;
using DividendAlertData.Services;
using DividendAlertData.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{
    // TODO [Authorize]
    [Route("api/[controller]")]
    public class CheckNewDividendsController : Controller
    {
        private readonly IMailSender _mailSender;
        private readonly IDividendsHtmlBuilder _dividendsHtmlBuilder;
        private readonly IDividendListBuilder _dividendListBuilder;

        public CheckNewDividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
        }



        [HttpGet]
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
        public async Task<IEnumerable<Dividend>> GetJsonAsync(string customStockList = null)
        {
            //"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            //"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2"

            return await _dividendListBuilder.ScrapeAndBuildDividendListAsync("https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos");
        }


    }
}
