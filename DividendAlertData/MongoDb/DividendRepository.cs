using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class DividendRepository : BaseMongoRepository<Dividend>, IDividendRepository
    {

        public DividendRepository(IConfiguration config) : base(config, "dividends")
        {
        }


        public async Task<IEnumerable<Dividend>> GetByStockNameAsync(string stockName)
        {
            var filter = Builders<Dividend>.Filter.Eq(d => d.StockName, stockName);

            var result = await collection.FindAsync(filter);

            return result.ToEnumerable();
        }

    }

}
