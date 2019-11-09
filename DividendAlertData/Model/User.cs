namespace DividendAlertData.Model
{
    public class User
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string JwtToken { get; set; }


        public string[] GetUserStockList()
        {
            // TODO get user stock list from database
            return new string[] { "ITSA", "BBSE", "CCRO", "RADL", "ABEV", "EGIE", "HGTX", "WEGE", "FLRY" };
        }

    }

}