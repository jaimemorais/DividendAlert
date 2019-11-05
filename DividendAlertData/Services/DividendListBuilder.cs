using DividendAlertData.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DividendAlertData.Services
{
    public class DividendListBuilder : IDividendListBuilder
    {
        public async Task<IEnumerable<Dividend>> ScrapeAndBuildDividendListAsync(string uri, string stock)
        {
            IList<Dividend> list = new List<Dividend>();



            using (HttpClient httpClient = new HttpClient())
            {
                string html = await httpClient.GetStringAsync(uri + stock);

                if (!string.IsNullOrEmpty(html))
                {
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    string json = htmlDoc.GetElementbyId("results").GetAttributeValue("value", "").Replace("&quot;", "\"");

                    JArray objList = (JArray)JsonConvert.DeserializeObject(json);

                    for (int i = 0; i < objList.Count; i++)
                    {



                        list.Add(new Dividend()
                        {
                            Stock = stock,
                            PaymentDate = objList[i]["pd"].ToString(),
                            Type = objList[i]["et"].ToString(),
                            Value = objList[i]["v"].ToString()
                        });
                    }

                }

            }


            return list;
        }
    }


}
