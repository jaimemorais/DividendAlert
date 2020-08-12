using System.Collections.Generic;

namespace DividendAlertData.Services
{

    public class Provent
    {
        public string resultAbsoluteValue { get; set; }
        public string dateCom { get; set; }
        public string paymentDividend { get; set; }
        public string code { get; set; }
        public string companyName { get; set; }
        public string normalizedName { get; set; }
        public int companyId { get; set; }
        public int type { get; set; }
        public object group { get; set; }
        public string typeDesc { get; set; }
        public int day { get; set; }
        public string date { get; set; }
        public string url { get; set; }
        public int CategoryType { get; set; }
    }

    public class ProventsPayment
    {
        public string resultAbsoluteValue { get; set; }
        public string dateCom { get; set; }
        public string paymentDividend { get; set; }
        public string code { get; set; }
        public string companyName { get; set; }
        public string normalizedName { get; set; }
        public int companyId { get; set; }
        public int type { get; set; }
        public object group { get; set; }
        public string typeDesc { get; set; }
        public int day { get; set; }
        public string date { get; set; }
        public string url { get; set; }
        public int CategoryType { get; set; }
    }

    public class Meeting
    {
        public string description { get; set; }
        public string code { get; set; }
        public string companyName { get; set; }
        public string normalizedName { get; set; }
        public int companyId { get; set; }
        public int type { get; set; }
        public string group { get; set; }
        public string typeDesc { get; set; }
        public int day { get; set; }
        public string date { get; set; }
        public string url { get; set; }
        public int CategoryType { get; set; }
    }

    public class EventDay
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<int> Types { get; set; }
    }

    public class Stock
{
    public int type { get; set; }
    public string companyName { get; set; }
    public bool Loaded { get; set; }
    public List<object> manuais { get; set; }
    public List<object> iPOs { get; set; }
    public List<Provent> Provents { get; set; }
    public List<ProventsPayment> proventsPayment { get; set; }
    public List<object> reports { get; set; }
    public List<Meeting> meetings { get; set; }
    public List<EventDay> eventDays { get; set; }
    public List<object> holidays { get; set; }
    public int TotalEvents { get; set; }
}


}
