using DividendAlertData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DividendAzureFunction
{
    public static class GetNewDividendsHtmlFunction
    {
        [FunctionName("GetNewDividendsHtmlFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetNewDividendsHtmlFunction processed a request.");

            string stocks = req.Query["stocks"];

            if (stocks == null)
            {
                return new BadRequestObjectResult("<html><p>Please pass the stocks on the query string or in the request body</p></html>");
            }
            else
            {
                string[] stockList = stocks.Split(';');
                string html = await new DividendsHtmlBuilder().GenerateHtmlAsync(stockList);
                return new OkObjectResult(html);
            }
        }
    }
}
