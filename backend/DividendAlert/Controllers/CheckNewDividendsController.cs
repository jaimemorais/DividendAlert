using DividendAlert.Mail;
using DividendAlertData;
using Microsoft.AspNetCore.Mvc;
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

            bool newDividends = !html.Contains(DividendsHtmlGenerator.NO_DIVIDENDS_FOR_TODAY);

            if (newDividends)
            {
                _mailSender.SendMail("jaimemorais@gmail.com", html);
            }

            return html;
        }


        [HttpGet]
        [Route("json")]
        public async Task<string> GetJsonAsync(string customStockList = null)
        {
            // TODO
            return "";
        }


    }
}
