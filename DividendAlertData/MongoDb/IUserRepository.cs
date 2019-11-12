using DividendAlertData.Model;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public interface IUserRepository : IMongoRepository<User>
    {

        Task<User> GetByEmailAsync(string email);
    }
}
