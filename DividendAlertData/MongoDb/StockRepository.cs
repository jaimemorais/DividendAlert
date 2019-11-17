using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;

namespace DividendAlertData.MongoDb
{
    public class StockRepository : BaseMongoRepository<Stock>, IStockRepository
    {

        public StockRepository(IConfiguration config) : base(config, "stocks")
        {
        }




    }

}
