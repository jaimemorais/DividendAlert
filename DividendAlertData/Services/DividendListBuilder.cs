using DividendAlertData.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DividendAlertData.Services
{
    public class DividendListBuilder : IDividendListBuilder
    {



        public async Task<IEnumerable<Dividend>> ScrapeAndBuildDividendListAsync(string dividendSiteToScrape, string stockName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(dividendSiteToScrape + stockName);

                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(html))
                    {
                        return ScrapeHtml(stockName, html);
                    }
                }
            }

            return Enumerable.Empty<Dividend>();
        }

        private static IEnumerable<Dividend> ScrapeHtml(string stockName, string html)
        {
            IList<Dividend> list = new List<Dividend>();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            var results = htmlDoc.GetElementbyId("results");

            if (results != null)
            {
                string json = results.GetAttributeValue("value", "").Replace("&quot;", "\"");
                JArray objList = (JArray)JsonConvert.DeserializeObject(json);

                foreach (JToken obj in objList)
                {
                    list.Add(new Dividend()
                    {
                        StockName = stockName,
                        ExDate = obj["ed"].ToString(),
                        PaymentDate = obj["pd"].ToString(),
                        Type = obj["et"].ToString(),
                        Value = obj["v"].ToString()
                    });
                }
            }

            return list;
        }
    }


}
