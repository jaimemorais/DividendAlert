using DividendAlertData.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public interface IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        Task InsertAsync(TEntity entity);

        Task ReplaceAsync(TEntity entity);

        Task<TEntity> GetByIdAsync(string id);

        Task<IList<TEntity>> GetAllAsync();
    }
}
