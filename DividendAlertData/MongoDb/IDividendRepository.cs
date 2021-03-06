﻿using DividendAlertData.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.MongoDb
{
    public interface IDividendRepository : IMongoRepository<Dividend>
    {

        Task<IEnumerable<Dividend>> GetByStockNameAsync(string stockName);

        Task<IEnumerable<Dividend>> GetByStockAsync(Dividend dividend);

        Task<IEnumerable<Dividend>> GetLastDaysDividends(int days);
    }
}
