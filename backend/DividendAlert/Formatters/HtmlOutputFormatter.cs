using Microsoft.AspNetCore.Mvc.Formatters;

namespace DividendAlert.Formatters
{
    public class HtmlOutputFormatter : StringOutputFormatter
    {
        public HtmlOutputFormatter()
        {
            SupportedMediaTypes.Add("text/html");
        }
    }
}