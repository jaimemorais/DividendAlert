using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DividendAlertData
{

    public static class DividendsHtmlGenerator
    {

        public const string NO_DIVIDENDS_FOR_TODAY = "No dividends for today";


        public static async Task<string> GenerateHtmlAsync(string[] stockList)
        {
            // TODO : http://siteempresas.bovespa.com.br/consbov/ExibeFatosRelevantesCvm.asp 

            string url = "http://www.dividendobr.com/";
            string htmlDividendoBr = GenerateHtml(url, await ScrapeHtmlAsync(stockList, url, "tclass"));

            url = "https://www.meusdividendos.com/comunicados/";
            string htmlMeusDividendos = GenerateHtml(url, await ScrapeHtmlAsync(stockList, url, "card mb-4"));


            bool noDividendsForToday =
                htmlDividendoBr.Contains(NO_DIVIDENDS_FOR_TODAY) && htmlMeusDividendos.Contains(NO_DIVIDENDS_FOR_TODAY);

            return noDividendsForToday
                ? $"<p>{NO_DIVIDENDS_FOR_TODAY}</p>"
                : htmlDividendoBr + "<br/><br/><br/><br/><hr/>" + htmlMeusDividendos;
        }


        private static string GenerateHtml(string url, string htmlContent)
        {
            string header = "<h2>Today New Dividends</h2> " +
                            $"<a href='{url}'>{url}</a>" +
                            "<br/>";

            if (!string.IsNullOrEmpty(htmlContent))
            {
                return header + htmlContent;
            }

            return header + $"<br/><p>{NO_DIVIDENDS_FOR_TODAY} in {url}</p>";
        }

        private static async Task<string> ScrapeHtmlAsync(string[] stockList, string url, string stockCssClass)
        {
            StringBuilder sbHtml = new StringBuilder();

            using (HttpClient httpClient = new HttpClient())
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string htmlPage = await response.Content.ReadAsStringAsync();

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(htmlPage);

                    HtmlNodeCollection stockNodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class,'{stockCssClass}')]");

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

            return sbHtml.ToString();
        }

    }
}