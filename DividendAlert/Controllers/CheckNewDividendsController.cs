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
    public class CheckNewDividendsController : Controller
    {

        [HttpGet]
        public async Task<string> GetAsync(string token)
        {   
            
            List<User> userList = new List<User>(); // TODO findAll
            
            foreach (User user in userList) 
            {
                string html = await NewDividendsHtmlGenerator.GenerateHtmlAsync(user.GetUserStockList());
                if (!string.IsNullOrEmpty(html)) 
                {
                    SendMail(user.Email, html);
                }
            }

            return "OK";
        }


        private void SendMail(string email, string html) 
        {
            // TODO
        }
        
    }
}
