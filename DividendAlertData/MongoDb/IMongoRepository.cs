using DividendAlertData.Model;
using System;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public interface IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        Task InsertAsync(TEntity entity);

        Task ReplaceAsync(TEntity entity);

        Task<TEntity> GetByIdAsync(Guid id);

    }
}
