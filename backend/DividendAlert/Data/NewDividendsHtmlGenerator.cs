using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace DividendAlert.Data
{

    public class NewDividendsHtmlGenerator
    {

        public static async Task<string> GenerateHtmlAsync(string[] stockList)
        {
            string html = await GetHtmlAsync(stockList, "http://www.dividendobr.com/", "tclass");

            html += "<br/><br/><br/><br/><hr/>";

            html += await GetHtmlAsync(stockList, "https://www.meusdividendos.com/anuncios-dividendos/", "timeline-item");

            return html;
        }


        private static async Task<string> GetHtmlAsync(string[] stockList, string url, string stockHtmlClass)
        {
            string header = "<h2>Today New Dividends</h2> " +                            
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
                return header + resultHtml;
            }

            return header + $"<br/><p>No dividends for today in {url}</p>";
        }
    }
}