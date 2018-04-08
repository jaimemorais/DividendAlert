using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DividendAlert.Data;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace DividendAlert.Controllers
{
    [Route("api/[controller]")]
    public class NewDividendsHtmlController : Controller
    {

        [HttpGet]
        [Produces("text/html")]
        public async Task<string> GetAsync(string user)
        {   
            string[] userStockList = UserData.GetUserStockList(user);          

            return await new NewDividendsHtmlGenerator().GenerateHtmlAsync(userStockList);            
        }

        
    }
}
