using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using DividendAlertData.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        public DividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder,
            IUserRepository userRepository)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
            _userRepository = userRepository;
        }



        [HttpGet]
        [Route("html")]
        [Produces("text/html")]
        public async Task<string> GetHtmlAsync(string customStockList = null)
        {


            User currentUser = new User(); // TODO get by id _userRepository.GetById();
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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = await _userRepository.GetByEmailAsync(identity.FindFirst("Email").Value);

            // TODO
            // user.GetUserStockList()


            //"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            //"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";

            // TODO create a hosted service to scrape all stocks listed on the database
            // Change here to read the db

            const string uri = "https://statusinvest.com.br/acoes/";

            return await _dividendListBuilder.ScrapeAndBuildDividendListAsync(uri, "ccro3");
        }


    }
}
