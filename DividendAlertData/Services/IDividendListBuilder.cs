using DividendAlertData.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DividendAlertData.Services
{
    public interface IDividendListBuilder
    {
        Task<IEnumerable<Dividend>> ScrapeAndBuildDividendListAsync(string uri, string stockName);
    }
}
