namespace DividendAlertData
{
    public class User
    {

        public string Email { get; set; }

        public string[] GetUserStockList()
        {
            // TODO get user stock list from database
            return new string[] { "ITSA", "BBSE", "CCRO", "RADL", "ABEV", "EGIE", "HGTX", "WEGE" };
        }

    }

}