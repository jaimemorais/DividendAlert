namespace DividendAlert.Data
{
    public class User
    {
        
        public string Email { get; set; }

        public string[] GetUserStockList() 
        {
           // TODO get user stock list from database
            return new string[] { "ODPV", "SULA" };
        }

    }

}