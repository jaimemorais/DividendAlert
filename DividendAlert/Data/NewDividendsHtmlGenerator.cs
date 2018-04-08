using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DividendAlert.Data
{
    
    public class NewDividendsHtmlGenerator
    {
        public async Task<string> GenerateHtmlAsync(string[] stockList) 
        {
            const string HEADER = "<p>Today New Dividends</p>";
            string resultHtml = HEADER;

            using (HttpClient httpClient = new HttpClient())            
            using (HttpResponseMessage response = await httpClient.GetAsync("http://www.dividendobr.com/")) 
            {
                if (response.IsSuccessStatusCode) 
                {
                    string htmlPage = await response.Content.ReadAsStringAsync();

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(htmlPage);

                    HtmlNodeCollection stockNodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(@class,'tclass')]");

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

            if (resultHtml != HEADER) 
            {
                return resultHtml;
            }

            return "<p>Nothing for today :(</p>";
        }
    }
}