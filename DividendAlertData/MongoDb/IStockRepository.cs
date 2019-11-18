using DividendAlertData.Model;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public interface IStockRepository : IMongoRepository<Stock>
    {

        Task<Stock> GetByNameAsync(string name);
    }
}
