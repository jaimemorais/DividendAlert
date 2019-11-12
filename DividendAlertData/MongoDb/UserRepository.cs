using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class UserRepository : BaseMongoRepository<User>, IUserRepository
    {

        public UserRepository(IConfiguration config) : base(config, "users")
        {
        }


        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);

            var result = await collection.FindAsync(filter);

            if (result != null && result.Current != null && result.Current.Count() == 1)
            {
                return result.Current.Single();
            }

            return null;
        }



    }

}
