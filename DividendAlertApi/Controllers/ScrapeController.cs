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
            // TODO better auth
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



        // TODO migrate to Azure AppService Webjobs https://docs.microsoft.com/en-us/azure/app-service/webjobs-create
        [HttpGet]
        [Route("scrape/{scrapeToken}")]
        public async Task<IActionResult> Scrape(string scrapeToken)
        {
            // TODO better auth
            if (!_config["ScrapeToken"].Equals(scrapeToken))
            {
                return Unauthorized();
            }

            try
            {
                _logger.LogInformation("Scraping started at " + DateTime.Now.ToString());

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
                            IEnumerable<Dividend> scrapedList = await _dividendListBuilder.ScrapeAndBuildDividendListAsync(_config["DividendSiteToScrape"], stockName);

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
            // TODO better auth
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
