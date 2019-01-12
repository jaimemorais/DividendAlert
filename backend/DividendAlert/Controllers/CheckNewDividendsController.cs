using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DividendAlert.Data;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DividendAlert.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CheckNewDividendsController : Controller
    {

        [HttpGet]
        [Produces("text/html")]
        public async Task<string> GetAsync(string token, string customStockList = null)
        {

            // TODO remove
            User currentUser = new User();
            string[] stockList = currentUser.GetUserStockList();

            
            if (customStockList != null)
            {
                stockList = customStockList.Split(";");
            }


            string html = await NewDividendsHtmlGenerator.GenerateHtmlAsync(stockList);
            if (!string.IsNullOrEmpty(html)) 
            {                
                return html;
            }

            return "<p>No dividends for today</p>";
        }


    }
}
