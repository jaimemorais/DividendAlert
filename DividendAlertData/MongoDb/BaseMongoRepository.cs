using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class BaseMongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        private IMongoDatabase database;
        private IMongoCollection<TEntity> collection;

        private IConfiguration _config;

        public BaseMongoRepository(IConfiguration config, string collectionName)
        {
            _config = config;

            ConfigDatabase();
            ConfigCollection(collectionName);
        }




        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);

            var result = await collection.FindAsync(filter);

            if (result != null && result.Current != null && result.Current.Count() == 1)
            {
                return result.Current.Single();
            }

            return null;
        }


        public async Task InsertAsync(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            await collection.InsertOneAsync(entity);
        }





        private void ConfigDatabase()
        {
            MongoClientSettings mongoClientSettings = new MongoClientSettings()
            {
                Server = new MongoServerAddress(_config["MongoHost"], int.Parse(_config["MongoPort"])),
                Credential = MongoCredential.CreateCredential(_config["MongoDatabaseName"], _config["MongoUser"], _config["MongoPassword"]),
                RetryWrites = false,
                RetryReads = false
            };

            var client = new MongoClient(mongoClientSettings);

            database = client.GetDatabase(_config["MongoDatabaseName"]);
        }

        private void ConfigCollection(string collectionName)
        {

            collection = database.GetCollection<TEntity>(collectionName);
        }

    }
}
