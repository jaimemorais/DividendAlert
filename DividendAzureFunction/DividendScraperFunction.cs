using DividendAlertData.Model;
using DividendAlertData.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAzureFunction
{
    public static class DividendScraperFunction
    {

        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer
        // https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/




        [FunctionName("DividendScraperFunction")]
        public static async Task Run([TimerTrigger("0 0 10 * * *")]TimerInfo myTimer, ILogger log)
        {


            ////"https://www.bussoladoinvestidor.com.br/guia-empresas/empresa/CCRO3/proventos"
            ////"http://fundamentus.com.br/proventos.php?papel=ABEV3&tipo=2";            

            // TODO scrape all stocks listed on the database and save new dividends to mongodb

            IEnumerable<Dividend> dividendList =
                await new DividendListBuilder().ScrapeAndBuildDividendListAsync("https://statusinvest.com.br/acoes/", "ccro3");



            log.LogInformation($"DividendScraperFunction executed at: {DateTime.Now}");
        }
    }
}
