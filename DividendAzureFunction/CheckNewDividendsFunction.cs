using HtmlAgilityPack;
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
                string html = await GenerateHtmlAsync(stockList);
                response = req.CreateResponse(HttpStatusCode.OK);                
                response.Content = new StringContent(html);                
            }


            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }



        public static async Task<string> GenerateHtmlAsync(string[] stockList)
        {
            string html = await GetHtmlAsync(stockList, "http://www.dividendobr.com/", "tclass");

            html += "<br/><br/><br/><br/><hr/>";

            html += await GetHtmlAsync(stockList, "https://www.meusdividendos.com/anuncios-dividendos/", "timeline-item");

            return html;
        }


        private static async Task<string> GetHtmlAsync(string[] stockList, string url, string stockHtmlClass)
        {
            string html = "<html><h2>Today New Dividends</h2> " +
                            $"<a href='{url}'>{url}</a>" +
                            "<br/>";

            string resultHtml = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string htmlPage = await response.Content.ReadAsStringAsync();

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(htmlPage);

                    HtmlNodeCollection stockNodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class,'{stockHtmlClass}')]");

                    foreach (string userStock in stockList)
                    {
                        foreach (HtmlNode stockNode in stockNodes)
                        {
                            if (stockNode.InnerHtml.Contains(userStock))
                            {
                                resultHtml += "<br/><br/><table>" + stockNode.InnerHtml + "</table>";
                            }
                        }
                    }


                }
            }

            if (!string.IsNullOrEmpty(resultHtml))
            {
                return html + resultHtml + "</html>";
            }

            return html + $"<br/><p>No dividends for today in {url}</p></html>";
        }
    


    }
}
