using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using DividendAlertData.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<DividendsController> _logger;
        private readonly IConfiguration _config;

        public DividendsController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder,
            IUserRepository userRepository, IDividendRepository dividendRepository, IStockRepository stockRepository,
            ILogger<DividendsController> logger, IConfiguration configuration)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
            _userRepository = userRepository;
            _dividendRepository = dividendRepository;
            _stockRepository = stockRepository;
            _logger = logger;
            _config = configuration;
        }



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
        [Route("scrape/{scrapeToken}")]
        [Produces("text/html")]
        [AllowAnonymous]
        public IActionResult Scrape(string scrapeToken)
        {
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Scraping started at " + DateTime.Now.ToString());


            // Get stock list for scrape
            const string STOCK_LIST = "ABCB4;AZUL4;ABEV3;AGRO3;ALPA3;ALPA4;ALSC3;ALUP11;AMAR3;ARZZ3;ATOM3;BAHI3;BAZA3;BBAS3;BBSE3;BBDC4;BBRK3;BIDI4;ANIM3;BEEF3;BMEB3;BMEB4;BMIN4;BMTO4;BOBR4;BOVA11;BPHA3;BRAP3;BRAP4;BRFS3;BRGE11;BRGE3;BRGE8;BRIN3;BRIV4;BRKM3;BRKM5;BRML3;BRPR3;BRSR6;BSLI3;BTOW3;BTTL3;BTTL4;BVMF3;CALI4;CARD3;CCRO3;CCXC3;CELP3;CELP6;CESP6;CGAS5;CIEL3;CLSC4;CMIG3;CMIG4;COCE5;CPFE3;CPLE3;CPLE6;CRDE3;CSAN3;CSMG3;CSNA3;CSRN5;CTAX3;CTIP3;CTKA4;CVCB3;CYRE3;DASA3;DIRR3;DIVO11;DTEX3;ECOR3;EEEL3;ELET3;ELET6;ELPL4;EMAE4;EMBR3;ENBR3;ENGI11;EQTL3;ESTC3;ESTR4;ETER3;EUCA4;EVEN3;EZTC3;FESA4;FHER3;FIBR3;FJTA4;FLRY3;FRAS3;FRIO3;GFSA3;GGBR3;GGBR4;GOAU3;GOAU4;GOLL4;GOVE11;GPCP3;GRND3;GSHP3;GUAR3;GUAR4;HAGA4;HBOR3;HGTX3;HYPE3;IDNT3;IGTA3;IMBI4;INEP3;INEP4;ISUS11;ITSA3;ITSA4;ITUB4;JBSS3;JHSF3;JSLG3;KEPL3;KLBN11;KROT3;LAME3;LAME4;LCAM3;LEVE3;LIGT3;LLIS3;LOGN3;LPSB3;LREN3;LUPA3;LUXM3;LUXM4;MAGG3;MDIA3;MGEL4;MGLU3;MILS3;MLFT4;MNDL3;MOAR3;MPLU3;MRFG3;MRVE3;MULT3;MYPK3;NAFG4;NATU3;ODPV3;OGXP3;OIBR3;OIBR4;PARC3;PATI3;PCAR4;PDGR3;PEAB3;PETR3;PETR4;PFRM3;PIBB11;PINE4;PMAM3;POMO3;POMO4;POSI3;PSSA3;PTBL3;PTNT3;ENAT3;QUAL3;RADL3;RAPT3;RAPT4;RCSL3;RDNI3;RENT3;RNEW11;ROMI3;RPMG3;RAIL3;SANB11;SANB3;SANB4;SAPR4;SBSP3;SCAR3;SEER3;SMLS3;SGAS4;SGPS3;SHOW3;SHUL4;SLCE3;SLED4;SMLE3;SMTO3;SSBR3;SULA11;SUZB5;SUZB6;TAEE11;TCNO4;TCSA3;TECN3;TELB4;TENE7;TGMA3;TIMP3;TOTS3;TOYB3;TRIS3;TRPL3;TRPL4;TRPN3;TUPY3;UCAS3;UGPA3;UNIP5;UNIP6;USIM3;USIM5;TIET11;VALE3;VALE5;VIVR3;VIVT3;VIVT4;VLID3;VULC3;VVAR11;WEGE3;WHRL3;WSON33";
            string[] stockList = STOCK_LIST.Split(";");


            // Fire and forget
            Parallel.ForEach(stockList, async (stockName) =>
            {
                IEnumerable<Dividend> scrapedList = await _dividendListBuilder.ScrapeAndBuildDividendListAsync(stockName);

                if (scrapedList.Any())
                {
                    Parallel.ForEach(scrapedList, async (scrapedDividend) =>
                    {
                        if (DateTime.TryParseExact(scrapedDividend.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime paymentDate) &&
                            (paymentDate.Year >= 2019) &&
                            !(await _dividendRepository.GetByStockAsync(scrapedDividend)).Any())
                        {
                            scrapedDividend.DateAdded = DateTime.Today;
                            await _dividendRepository.InsertAsync(scrapedDividend);
                            _logger.LogInformation($"New dividend for {stockName} added. Payment Date = {paymentDate.ToShortDateString()}");
                        }
                    });
                }
            });


            return Ok();
        }


    }
}
