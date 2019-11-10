using DividendAlertData.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DividendAlertData.MongoDb
{
    public interface IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
        IList<TEntity>
        SearchFor(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        TEntity GetById(Guid id);
    }
}
