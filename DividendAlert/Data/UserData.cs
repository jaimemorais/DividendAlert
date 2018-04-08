namespace DividendAlert.Data
{
    public class UserData
    {
        
        public static string[] GetUserStockList(string user) 
        {
           // TODO get user stock list from database
            return new string[] { "ODPV", "SULA" };
        }

    }

}