using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    public class GeneralData
    {
        public GeneralData(string deliveryType, string buySell, string transactionName)
        {
            this.DeliveryType = deliveryType;
            this.BuySell = buySell;
            this.TransactionName = transactionName;
        }

        public GeneralData(){}

        public string DeliveryType { get; set; }

        public string BuySell { get; set; }

        public string TransactionName { get; set; }

        [XmlIgnore]
        public bool TransactionNameSpecified
        {
            get { return !string.IsNullOrEmpty(this.TransactionName); }
        }
       
    }
}