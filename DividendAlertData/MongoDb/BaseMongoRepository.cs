using DividendAlertData.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public abstract class BaseMongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        protected IMongoCollection<TEntity> collection;

        protected BaseMongoRepository(string connectionString, string databaseName, string collectionName)
        {
            MongoClient client = new MongoClient(connectionString);

            IMongoDatabase database = client.GetDatabase(databaseName);

            collection = database.GetCollection<TEntity>(collectionName);
        }


        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await (await collection.FindAsync(_ => true)).ToListAsync();
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

        public async Task ReplaceAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            await collection.ReplaceOneAsync(filter, entity);
        }




    }
}
