using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    public class SettlementData
    {
        public SettlementData(){}

        public SettlementData(double quantity, string quantityUnit, double price, double priceMargin, double marketPriceMultiplicator, string priceVolumeUnit,
            string currency, string currencySource, string resolution, string settlementRule, string flexibleTimeSeries)
        {
            Quantity = quantity;
            QuantityUnit = quantityUnit;
            Price = price;
            PriceMargin = priceMargin;
            PriceVolumeUnit = priceVolumeUnit;
            Currency = currency;
            CurrencySource = currencySource;
            Resolution = resolution;
            SettlementRule = settlementRule;
            FlexibleTimeSeries = flexibleTimeSeries;
        }

        public double Quantity { get; set; }

        public string QuantityUnit { get; set; }
        
        public double Price { get; set; }
        
        public double PriceMargin { get; set; }

        public double MarketPriceMultiplicator { get; set; }

        public string PriceVolumeUnit { get; set; }

        public string Currency { get; set; }

        public string CurrencySource { get; set; }

        public string Resolution { get; set; }

        public string SettlementRule { get; set; }

        public string FlexibleTimeSeries { get; set; }

        [XmlIgnore]
        public bool QuantitySpecified { get { return  this.Quantity != 0F; }  }

        [XmlIgnore]
        public bool QuantityUnitSpecified
        {
            get { return !string.IsNullOrEmpty(this.QuantityUnit); }
        }

        
    }
    
}