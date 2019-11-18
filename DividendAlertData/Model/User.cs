namespace DividendAlertData.Model
{
    public class User : BaseMongoEntity
    {

        public string Email { get; set; }

        public string Password { get; set; }

        public string JwtToken { get; set; }

        public string PasswordResetCode { get; set; }

        public string StockList { get; set; }

    }

}