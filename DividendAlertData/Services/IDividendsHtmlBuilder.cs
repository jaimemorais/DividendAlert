using System.Threading.Tasks;

namespace DividendAlertData.Services
{
    public interface IDividendsHtmlBuilder
    {
        Task<string> GenerateHtmlAsync(string[] stockList);
    }
}
