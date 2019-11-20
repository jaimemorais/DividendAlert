using DividendAlertData.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class DividendRepository : BaseMongoRepository<Dividend>, IDividendRepository
    {

        public DividendRepository(string connectionString, string databaseName) : base(connectionString, databaseName, "dividends")
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
