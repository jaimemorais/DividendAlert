using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DividendAlert.Controllers
{
    [Route("api/[controller]")]
    public class CheckDividendController : Controller
    {
        // GET api/CheckDividend
        [HttpGet]
        public async Task<string> GetAsync()
        {
            
            using (HttpClient httpClient = new HttpClient())            
            using (HttpResponseMessage response = await httpClient.GetAsync("http://www.dividendobr.com/")) 
            {
                if (response.IsSuccessStatusCode) 
                {
                    string htmlPage = await response.Content.ReadAsStringAsync();

                    string[] myStockList = new string[] { "ITUB" };
                    foreach(var stock in myStockList) 
                    {
                        string dividendDate = "";
                        if (htmlPage.Contains(stock))
                        {
                            dividendDate = "[TODO]"; // TODO
                        }

                        return stock + " - " + dividendDate;
                    }
                    

                }
            }        


            return "nothing found";
        }

        
    }
}
