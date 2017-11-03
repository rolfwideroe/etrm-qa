using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace TestLiveCurveScheduling
{
    [XmlRoot("PriceFeed")]
    public class PriceBoardCustomPriceFeed
    {
        [XmlElement("param")]
        public PriceBoardCustomPriceFeedParameters Parameters { get; set; }

        [XmlElement("price")]
        public PriceBoardCustomPriceFeedPrice[] Prices { get; set; }
    }

    [XmlRoot("param")]
    public class PriceBoardCustomPriceFeedParameters
    {
        [XmlAttribute]
        public string LoadType { get; set; }
        [XmlAttribute]
        public string Venue { get; set; }
        [XmlAttribute]
        public string Couterparty { get; set; }
        [XmlAttribute]
        public string Commodity { get; set; }
        [XmlAttribute]
        public string RefArea { get; set; }
        [XmlAttribute]
        public string TimeZone { get; set; }
        [XmlAttribute]
        public string Feed { get; set; }
        [XmlAttribute]
        public bool IsOption { get; set; }
        [XmlAttribute]
        public bool IsSwap { get; set; }
    }

    [XmlRoot("price")]
    public class PriceBoardCustomPriceFeedPrice
    {
        [XmlAttribute]
        public string LoadType { get; set; }
        [XmlAttribute]
        public string Ticker { get; set; }

        [XmlAttribute]
        public DateTime From { get; set; }
        [XmlAttribute]
        public DateTime To { get; set; }
        [XmlAttribute]
        public string PeriodType { get; set; }

        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Last { get; set; }
        public double Close { get; set; }
    }
}
