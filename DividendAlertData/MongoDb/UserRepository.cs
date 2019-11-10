using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;

namespace DividendAlertData.MongoDb
{
    public class UserRepository : BaseMongoRepository<User>, IUserRepository
    {

        public UserRepository(IConfiguration config) : base(config, "users")
        {

        }
    }

}
