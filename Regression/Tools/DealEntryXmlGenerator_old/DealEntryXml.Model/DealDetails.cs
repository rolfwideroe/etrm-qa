using System;
using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    public class DealDetails
    {
        public DealDetails(){}

        public DealDetails(string trader, string status, DateTime tradeDate)
        {
            Trader = trader;
            Status = status;
            TradeDate = tradeDate;
        }

        public string Trader { get; set; }

        public string Status { get; set; }

        [XmlIgnore]
        public DateTime TradeDate { get; set; }

        [XmlElement(ElementName = "TradeDate")]
        public string XmlTradeDate
        {
            get { return this.TradeDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.TradeDate = DateTime.Parse(value); }
        }

        public bool XmlTradeDateSpecified { get { return this.TradeDate != DateTime.MinValue; } }

    }
}