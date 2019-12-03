using DividendAlertData.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public class DividendRepository : BaseMongoRepository<Dividend>, IDividendRepository
    {

        public DividendRepository(string connectionString, string databaseName) : base(connectionString, databaseName, "dividends")
        {
        }


        public async Task<IEnumerable<Dividend>> GetByStockNameAsync(string stockName)
        {
            var filter = Builders<Dividend>.Filter.Eq(d => d.StockName, stockName);

            var result = await collection.FindAsync(filter);

            return result.ToEnumerable();
        }

        public async Task<IEnumerable<Dividend>> GetByStockAsync(Dividend dividend)
        {
            FilterDefinition<Dividend> filterName = Builders<Dividend>.Filter.Eq(s => s.StockName, dividend.StockName);
            FilterDefinition<Dividend> filterPaymentDate = Builders<Dividend>.Filter.Eq(s => s.PaymentDate, dividend.PaymentDate);
            FilterDefinition<Dividend> filterExDate = Builders<Dividend>.Filter.Eq(s => s.ExDate, dividend.ExDate);
            FilterDefinition<Dividend> filterType = Builders<Dividend>.Filter.Eq(s => s.Type, dividend.Type);

            FilterDefinition<Dividend> combineFilters = Builders<Dividend>.Filter.And(
                filterName,
                filterType,
                filterExDate,
                filterPaymentDate);


            var result = await collection.FindAsync(combineFilters);

            return result.ToEnumerable();
        }

        public async Task<IEnumerable<Dividend>> GetLastDaysDividends(int days)
        {
            var filter = Builders<Dividend>.Filter.Gte(d => d.DateAdded, DateTime.Now.AddDays(-days));

            var result = await collection.FindAsync(filter);

            return result.ToEnumerable();
        }

    }

}
