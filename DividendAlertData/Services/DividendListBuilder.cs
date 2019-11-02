using DividendAlertData.Model;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DividendAlertData.Services
{
    public class DividendListBuilder : IDividendListBuilder
    {
        public async Task<IEnumerable<Dividend>> ScrapeAndBuildDividendListAsync(string uri)
        {
            IList<Dividend> list = new List<Dividend>();



            using (HttpClient httpClient = new HttpClient())
            {
                string html = await httpClient.GetStringAsync(uri);

                if (!string.IsNullOrEmpty(html))
                {
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    HtmlNodeCollection stockNodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class,'ng-scope')]");

                    list.Add(new Dividend()
                    {

                    });
                }

            }


            return list;
        }
    }


}
