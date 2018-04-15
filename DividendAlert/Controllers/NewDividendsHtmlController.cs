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
        public async Task<string> GetAsync(string userName)
        {   
            User user = new User(); // TODO findByUserName

            string[] userStockList = user.GetUserStockList();          

            return await NewDividendsHtmlGenerator.GenerateHtmlAsync(userStockList);            
        }

        
    }
}
