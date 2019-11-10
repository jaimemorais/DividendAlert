using DividendAlertData.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DividendAlertData.MongoDb
{
    public class BaseMongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        private IMongoDatabase database;
        private MongoCollection<TEntity> collection;

        private IConfiguration _config;

        public BaseMongoRepository(IConfiguration config, string collectionName)
        {
            _config = config;
            GetDatabase();
            GetCollection(collectionName);
        }

        public bool Insert(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            return collection.Insert(entity).DocumentsAffected > 0;
        }

        public bool Update(TEntity entity)
        {
            if (entity.Id == null)
                return Insert(entity);

            return collection
                .Save(entity)
                .DocumentsAffected > 0;
        }

        public bool Delete(TEntity entity)
        {
            return collection
                .Remove(Query.EQ("id", entity.Id))
                .DocumentsAffected > 0;
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            // TODO
            /*return collection
                .AsQueryable<TEntity>()
                .Where(predicate.Compile())
                .ToList();*/
            return null;
        }

        public IList<TEntity> GetAll()
        {
            return collection.FindAllAs<TEntity>().ToList();
        }

        public TEntity GetById(Guid id)
        {
            return collection.FindOneByIdAs<TEntity>(id);
        }


        private void GetDatabase()
        {
            var client = new MongoClient(_config["MongoConnection"]);
            database = client.GetDatabase(_config["MongoDatabaseName"]);
        }

        private void GetCollection(string collectionName)
        {
            collection = (MongoCollection<TEntity>)database.GetCollection<TEntity>(collectionName);
        }

    }
}
