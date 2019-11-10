namespace DividendAlertData.Model
{
    public class Dividend : BaseMongoEntity
    {
        public string Stock { get; set; }
        public string Type { get; set; }
        public string ExDate { get; set; }
        public string PaymentDate { get; set; }
        public string Value { get; set; }
    }

}
