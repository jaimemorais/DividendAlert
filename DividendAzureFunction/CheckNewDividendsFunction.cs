using DividendAlertData;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DividendAzureFunction
{
    public static class CheckNewDividendsFunction
    {
        [FunctionName("CheckNewDividendsFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("CheckNewDividendsFunction processed a request.");

            string stocks = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "stocks", true) == 0)
                .Value;


            HttpResponseMessage response;

            if (stocks == null)
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Content = new StringContent("<html><p>Please pass the stocks on the query string or in the request body</p></html>");
            }
            else
            {
                string[] stockList = stocks.Split(';');
                string html = await NewDividendsHtml.GenerateHtmlAsync(stockList);
                response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(html);
            }


            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }




    }
}
