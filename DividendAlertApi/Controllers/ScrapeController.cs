using Dasync.Collections;
using DividendAlert.Services.Mail;
using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using DividendAlertData.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAlert.Controllers
{


    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly IMailSender _mailSender;
        private readonly IDividendsHtmlBuilder _dividendsHtmlBuilder;
        private readonly IDividendListBuilder _dividendListBuilder;
        private readonly IDividendRepository _dividendRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<DividendsController> _logger;
        private readonly IConfiguration _config;


        public ScrapeController(IMailSender mailSender, IDividendsHtmlBuilder dividendsHtmlBuilder, IDividendListBuilder dividendListBuilder,
            IDividendRepository dividendRepository, IStockRepository stockRepository,
            ILogger<DividendsController> logger, IConfiguration configuration)
        {
            _mailSender = mailSender;
            _dividendsHtmlBuilder = dividendsHtmlBuilder;
            _dividendListBuilder = dividendListBuilder;
            _dividendRepository = dividendRepository;
            _stockRepository = stockRepository;
            _logger = logger;
            _config = configuration;
        }



        [HttpGet]
        [Route("html/{scrapeToken}")]
        [Produces("text/html")]
        public async Task<IActionResult> GetHtmlAsync(string scrapeToken)
        {
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }


            string[] stockList = "ABEV3;BBSE3;CCRO3;EGIE3;FLRY3;HGTX3;ITSA3;ITSA4;RADL3;WEGE3".Split(";");

            string html = await _dividendsHtmlBuilder.GenerateHtmlAsync(stockList);

            bool hasNewDividends = !html.Contains(Constants.NO_DIVIDENDS_FOR_TODAY);

            if (hasNewDividends)
            {
                _mailSender.SendMail("jaimemorais@gmail.com", "New Dividend Alert", html);
            }

            return Ok(html);
        }


        [HttpGet]
        [Route("scrape/{scrapeToken}")]
        public async Task<IActionResult> Scrape(string scrapeToken)
        {
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            try
            {
                _logger.LogInformation("Scraping started at " + DateTime.Now.ToString());


                // Create stock documents
                ////const string STOCK_LIST = "ABCB4;AZUL4;ABEV3;AGRO3;ALPA3;ALPA4;ALSC3;ALUP11;AMAR3;ARZZ3;ATOM3;BAHI3;BAZA3;BBAS3;BBSE3;BBDC4;BBRK3;BIDI4;ANIM3;BEEF3;BMEB3;BMEB4;BMIN4;BMTO4;BOBR4;BOVA11;BPHA3;BRAP3;BRAP4;BRFS3;BRGE11;BRGE3;BRGE8;BRIN3;BRIV4;BRKM3;BRKM5;BRML3;BRPR3;BRSR6;BSLI3;BTOW3;BTTL3;BTTL4;BVMF3;CALI4;CARD3;CCRO3;CCXC3;CELP3;CELP6;CESP6;CGAS5;CIEL3;CLSC4;CMIG3;CMIG4;COCE5;CPFE3;CPLE3;CPLE6;CRDE3;CSAN3;CSMG3;CSNA3;CSRN5;CTAX3;CTIP3;CTKA4;CVCB3;CYRE3;DASA3;DIRR3;DIVO11;DTEX3;ECOR3;EEEL3;ELET3;ELET6;ELPL4;EMAE4;EMBR3;ENBR3;ENGI11;EQTL3;ESTC3;ESTR4;ETER3;EUCA4;EVEN3;EZTC3;FESA4;FHER3;FIBR3;FJTA4;FLRY3;FRAS3;FRIO3;GFSA3;GGBR3;GGBR4;GOAU3;GOAU4;GOLL4;GOVE11;GPCP3;GRND3;GSHP3;GUAR3;GUAR4;HAGA4;HBOR3;HGTX3;HYPE3;IDNT3;IGTA3;IMBI4;INEP3;INEP4;ISUS11;ITSA3;ITSA4;ITUB4;JBSS3;JHSF3;JSLG3;KEPL3;KLBN11;KROT3;LAME3;LAME4;LCAM3;LEVE3;LIGT3;LLIS3;LOGN3;LPSB3;LREN3;LUPA3;LUXM3;LUXM4;MAGG3;MDIA3;MGEL4;MGLU3;MILS3;MLFT4;MNDL3;MOAR3;MPLU3;MRFG3;MRVE3;MULT3;MYPK3;NAFG4;NATU3;ODPV3;OGXP3;OIBR3;OIBR4;PARC3;PATI3;PCAR4;PDGR3;PEAB3;PETR3;PETR4;PFRM3;PIBB11;PINE4;PMAM3;POMO3;POMO4;POSI3;PSSA3;PTBL3;PTNT3;ENAT3;QUAL3;RADL3;RAPT3;RAPT4;RCSL3;RDNI3;RENT3;RNEW11;ROMI3;RPMG3;RAIL3;SANB11;SANB3;SANB4;SAPR4;SBSP3;SCAR3;SEER3;SMLS3;SGAS4;SGPS3;SHOW3;SHUL4;SLCE3;SLED4;SMLE3;SMTO3;SSBR3;SULA11;SUZB5;SUZB6;TAEE11;TCNO4;TCSA3;TECN3;TELB4;TENE7;TGMA3;TIMP3;TOTS3;TOYB3;TRIS3;TRPL3;TRPL4;TRPN3;TUPY3;UCAS3;UGPA3;UNIP5;UNIP6;USIM3;USIM5;TIET11;VALE3;VALE5;VIVR3;VIVT3;VIVT4;VLID3;VULC3;VVAR11;WEGE3;WHRL3;WSON33";
                ////string[] inserStockList = STOCK_LIST.Split(";");
                ////foreach (string sn in inserStockList)
                ////{
                ////    if ((await _stockRepository.GetByNameAsync(sn)).Equals(null))
                ////    {
                ////        await _stockRepository.InsertAsync(new Stock() { Id = new Guid(), Name = sn });
                ////    }
                ////}


                IList<Stock> stockDbList = await _stockRepository.GetAllAsync();
                
                string[] stockList = stockDbList.Select(s => s.Name).ToArray();
                // To debug just one stock 
                ////stockList = new string[] { "ESTC3" };

                string errors = null;


                await stockList.ParallelForEachAsync(
                    async stockName =>
                    {
                        try
                        {
                            IEnumerable<Dividend> scrapedList = await _dividendListBuilder.ScrapeAndBuildDividendListAsync(stockName);

                            if (scrapedList.Any())
                            {
                                Parallel.ForEach(scrapedList, async (scrapedDividend) =>
                                {
                                    await CheckAndInsertDividendAsync(stockName, scrapedDividend);
                                });
                            }
                        }
                        catch (Exception scrapeEx)
                        {
                            errors += $"Error scraping {stockName} : {scrapeEx.Message}; ";
                        }
                    }
                );



                // Fire and forget
                /*Parallel.ForEach(stockList, async (stockName) =>
                {
                    try
                    {
                        IEnumerable<Dividend> scrapedList = await _dividendListBuilder.ScrapeAndBuildDividendListAsync(stockName);

                        if (scrapedList.Any())
                        {
                            Parallel.ForEach(scrapedList, async (scrapedDividend) =>
                            {
                                await CheckAndInsertDividendAsync(stockName, scrapedDividend);
                            });
                        }
                    }
                    catch (Exception scrapeEx)
                    {
                        errors += $"Error scraping {stockName} : {scrapeEx.Message}; ";
                    }
                });*/


                if (errors != null)
                {
                    _mailSender.SendMail("jaimemorais@gmail.com", $"Dividend Alert Scraping Errors {DateTime.Now}", errors);
                    return StatusCode(500);
                }


                return Ok("Response : 200 - OK - Scrape completed without errors.");
            }
            catch (Exception ex)
            {
                _mailSender.SendMail("jaimemorais@gmail.com", "Dividend Alert Scraping Error", ex.ToString());
                return StatusCode(500);
            }
        }


        private async Task CheckAndInsertDividendAsync(string stockName, Dividend scrapedDividend)
        {
            bool paymentUndefined = !(DateTime.TryParseExact(scrapedDividend.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime paymentDate));

            if (!paymentUndefined && paymentDate.Year >= 2019)
            {
                bool alreadyAdded = (await _dividendRepository.GetByStockAsync(scrapedDividend)).Any();

                if (!alreadyAdded)
                {
                    scrapedDividend.DateAdded = DateTime.Today;
                    await _dividendRepository.InsertAsync(scrapedDividend);
                    _logger.LogInformation($"New dividend for {stockName} added. Payment Date = {scrapedDividend.PaymentDate}");
                }
            }
        }


        [HttpGet]
        [Route("lastAdded/{scrapeToken}/{days}")]
        [Produces("text/html")]
        public async Task<IActionResult> GetLastAddedDividends(string scrapeToken, int days)
        {
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            IEnumerable<Dividend> list = await _dividendRepository.GetLastDaysDividends(days);
            IOrderedEnumerable<Dividend> orderedList = list.ToList().OrderByDescending(d => d.DateAdded);

            string html = $"<h3>Last {days} days dividends : </h3></br>";
            foreach (Dividend d in orderedList)
            {
                html += $"<p> {d.StockName} - " +
                    $"DateAdded : {d.DateAdded.ToShortDateString()} - " +
                    $"Type : {d.Type} - " +
                    $"Payment Date : {d.PaymentDate} - " +
                    $"Value : {d.Value} " +
                    $"</p>";
            }

            return Ok(html);
        }

    }
}
