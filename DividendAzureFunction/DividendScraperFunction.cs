using DividendAlertData.Model;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAzureFunction
{
    public static class DividendScraperFunction
    {


        [FunctionName("DividendScraperFunction")] // 19 pm every week day
        public static async Task Run([TimerTrigger("0 0 19 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            //// https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer
            //// https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/

            log.LogInformation($"DividendScraperFunction started at: {DateTime.Now}");


            string mongoConnectionString = Environment.GetEnvironmentVariable("MongoConnectionString");
            string mongoDatabase = Environment.GetEnvironmentVariable("MongoDatabase");

            IDividendRepository dividendRepository = new DividendRepository(mongoConnectionString, mongoDatabase);
            IStockRepository stockRepository = new StockRepository(mongoConnectionString, mongoDatabase);
            IList<DividendAlertData.Model.Stock> stockList = await stockRepository.GetAllAsync();

            DividendListBuilder dividendListBuilder = new DividendListBuilder();


            foreach (DividendAlertData.Model.Stock stock in stockList)
            {
                IEnumerable<Dividend> scrapedList = 
                    await dividendListBuilder.ScrapeAndBuildDividendListAsync(Environment.GetEnvironmentVariable("DividendSiteToScrape"), stock.Name);

                foreach (Dividend scrapedDividend in scrapedList)
                {
                    if (!(await dividendRepository.GetByStockAsync(scrapedDividend)).Any())
                    {
                        scrapedDividend.DateAdded = DateTime.Today;
                        await dividendRepository.InsertAsync(scrapedDividend);
                    }
                }

            }

            log.LogInformation($"DividendScraperFunction finished at: {DateTime.Now}");
        }
    }
}
