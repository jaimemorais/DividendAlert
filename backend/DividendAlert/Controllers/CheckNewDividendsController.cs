using DividendAlert.Mail;
using DividendAlertData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{
    // TODO [Authorize]
    [Route("api/[controller]")]
    public class CheckNewDividendsController : Controller
    {
        private readonly IMailSender _mailSender;

        public CheckNewDividendsController(IMailSender mailSender)
        {
            _mailSender = mailSender;
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


            string html = await DividendsHtmlGenerator.GenerateHtmlAsync(stockList);

            bool hasNewDividends = !html.Contains(DividendsHtmlGenerator.NO_DIVIDENDS_FOR_TODAY);

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
            List<Dividend> dividendList = new List<Dividend>();

            using (HttpClient httpClient = new HttpClient())
            {
                string html =
                    await httpClient.GetStringAsync("http://www.b3.com.br/pt_br/produtos-e-servicos/negociacao/renda-variavel/empresas-listadas.htm?codigo=7617");
            }


            return dividendList;
        }


    }
}
