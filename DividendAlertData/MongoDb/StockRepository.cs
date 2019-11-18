using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class StockRepository : BaseMongoRepository<Stock>, IStockRepository
    {

        public StockRepository(IConfiguration config) : base(config, "stocks")
        {
        }


        public async Task<Stock> GetByNameAsync(string name)
        {
            var filter = Builders<Stock>.Filter.Eq(s => s.Name, name);

            var result = await collection.FindAsync(filter);

            if (result != null && result.Current != null && result.Current.Count() == 1)
            {
                return result.Current.Single();
            }

            return null;
        }

    }

}
