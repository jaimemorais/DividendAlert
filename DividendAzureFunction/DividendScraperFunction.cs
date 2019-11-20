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
        ////"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
        ////"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";            
        private const string DIVIDEND_SITE_URI = "https://statusinvest.com.br/acoes/";



        [FunctionName("DividendScraperFunction")]
        public static async Task Run([TimerTrigger("0 0 10 * * *")]TimerInfo myTimer, ILogger log)
        {
            //// https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer
            //// https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/

            log.LogInformation($"DividendScraperFunction started at: {DateTime.Now}");


            string connectionString = ""; // TODO read config
            string databaseName = ""; // TODO read config

            IDividendRepository dividendRepository = new DividendRepository(connectionString, databaseName);
            IStockRepository stockRepository = new StockRepository(connectionString, databaseName);
            IList<Stock> stockList = await stockRepository.GetAllAsync();

            foreach (Stock stock in stockList)
            {
                IEnumerable<Dividend> scrapedList =
                    await new DividendListBuilder().ScrapeAndBuildDividendListAsync(DIVIDEND_SITE_URI, stock.Name);

                foreach (Dividend scrapedDividend in scrapedList)
                {
                    if (!(await dividendRepository.GetByStockAsync(scrapedDividend)).Any())
                    {
                        await dividendRepository.InsertAsync(scrapedDividend);
                    }
                }

            }

            log.LogInformation($"DividendScraperFunction finished at: {DateTime.Now}");
        }
    }
}
