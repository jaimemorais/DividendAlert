using DividendAlert.Data;
using DividendAlert.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class CheckNewDividendsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IMailSender _mailSender;

        public CheckNewDividendsController(IConfiguration config, IMailSender mailSender)
        {
            _config = config;
            _mailSender = mailSender;
        }



        [HttpGet]
        [Produces("text/html")]
        public async Task<string> GetAsync(string customStockList = null)
        {
            // TODO remove
            User currentUser = new User();
            string[] stockList = currentUser.GetUserStockList();


            if (customStockList != null)
            {
                stockList = customStockList.Split(";");
            }


            string html = await NewDividendsHtmlGenerator.GenerateHtmlAsync(stockList);
            if (!string.IsNullOrEmpty(html))
            {
                _mailSender.SendMail("jaimemorais@gmail.com", html);

                return html;
            }

            return "<p>No dividends for today</p>";
        }


    }
}
