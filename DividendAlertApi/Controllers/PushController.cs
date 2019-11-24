using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DividendAlertApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {

        private readonly IDividendRepository _dividendRepository;


        public PushController(IDividendRepository dividendRepository)
        {
            _dividendRepository = dividendRepository;
        }


        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> Send()
        {
            throw new NotImplementedException();

        }

    }
}