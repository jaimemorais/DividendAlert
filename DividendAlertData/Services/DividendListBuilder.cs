using DividendAlertData.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;
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

            HtmlDocument htmlDocEvents = new HtmlDocument();
            string divHtml = GetHtmlFirstDivByClass(htmlDoc, "event-list w-100");
            if (divHtml == null)
            {
                return list;
            }
            htmlDocEvents.LoadHtml(divHtml);

            
            var input = htmlDocEvents.DocumentNode.SelectNodes("//input");
                        
            if (input != null)
            {
                string json = input[0].GetAttributeValue("value", "").Replace("&quot;", "\"");
                
                Stock stock = JsonConvert.DeserializeObject<Stock>(json); 

                foreach (Provent provent in stock.Provents)
                {
                    list.Add(new Dividend()
                    {
                        StockName = provent.code,
                        ExDate = provent.dateCom,
                        PaymentDate = provent.date,
                        Type = provent.typeDesc,
                        Value = provent.resultAbsoluteValue
                    });
                }

                /*JArray objList = (JArray)JsonConvert.DeserializeObject(json);
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
                }*/
            }

            return list;
        }

        private static string GetHtmlFirstDivByClass(HtmlDocument htmlDoc, string classToGet)
        {
            HtmlNodeCollection divs = htmlDoc.DocumentNode.SelectNodes("//div[@class='" + classToGet + "']");
            if (divs != null && divs.Count > 0)
            {
                return divs[0].OuterHtml;
            }

            return null;
        }
    }


}
