using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DividendAlertData
{

    public static class NewDividendsHtml
    {

        public const string NO_DIVIDENDS_FOR_TODAY = "No dividends for today";


        public static async Task<string> GenerateHtmlAsync(string[] stockList)
        {
            string htmlDividendoBr = await GetHtmlAsync(stockList, "http://www.dividendobr.com/", "tclass");
            string htmlMeusDividendos = await GetHtmlAsync(stockList, "https://www.meusdividendos.com/comunicados/", "card mb-4");

            bool noDividendsForToday =
                htmlDividendoBr.Contains(NO_DIVIDENDS_FOR_TODAY) && htmlMeusDividendos.Contains(NO_DIVIDENDS_FOR_TODAY);

            return noDividendsForToday
                ? $"<p>{NO_DIVIDENDS_FOR_TODAY}</p>"
                : htmlDividendoBr + "<br/><br/><br/><br/><hr/>" + htmlMeusDividendos;
        }


        private static async Task<string> GetHtmlAsync(string[] stockList, string url, string stockHtmlClass)
        {
            string header = "<h2>Today New Dividends</h2> " +
                            $"<a href='{url}'>{url}</a>" +
                            "<br/>";

            StringBuilder sbHtml = new StringBuilder();

            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string htmlPage = await response.Content.ReadAsStringAsync();

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(htmlPage);

                    HtmlNodeCollection stockNodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class,'{stockHtmlClass}')]");

                    if (!stockNodes.Any())
                    {
                        return $"<br/><p>Cannot read {url} source.</p>";
                    }

                    foreach (string userStock in stockList)
                    {
                        foreach (HtmlNode stockNode in stockNodes)
                        {
                            if (stockNode.InnerHtml.Contains(userStock))
                            {
                                sbHtml.Append("<br/><br/><table>" + stockNode.InnerHtml + "</table>");
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(sbHtml.ToString()))
            {
                return header + sbHtml.ToString();
            }

            return header + $"<br/><p>{NO_DIVIDENDS_FOR_TODAY} in {url}</p>";
        }
    }
}