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
        public async Task<string> GetAsync(string token)
        {   
            
            // TODO get logged user
            User currentUser = new User();            
            
            string html = await NewDividendsHtmlGenerator.GenerateHtmlAsync(currentUser.GetUserStockList());
            if (!string.IsNullOrEmpty(html)) 
            {
                //SendMail(user.Email, html);
                return html;
            }

            return "<p>No dividends for today</p>";
        }


        private void SendMail(string email, string html) 
        {
            // TODO
        }
        
    }
}
