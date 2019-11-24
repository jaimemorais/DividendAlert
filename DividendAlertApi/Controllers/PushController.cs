using DividendAlertData.MongoDb;
using Microsoft.AspNetCore.Mvc;

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



    }
}