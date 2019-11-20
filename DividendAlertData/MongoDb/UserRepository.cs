using DividendAlertData.Model;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class UserRepository : BaseMongoRepository<User>, IUserRepository
    {

        public UserRepository(string connectionString, string databaseName) : base(connectionString, databaseName, "users")
        {
        }


        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);

            return await collection.Find(filter).FirstOrDefaultAsync();
        }



    }

}
