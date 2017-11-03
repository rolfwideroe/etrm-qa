using System;
using System.Xml.Serialization;

namespace DealEntryXml.Model
{
    public class InstrumentData
    {
        public InstrumentData(){}

        public InstrumentData(string executionVenue, string priceBasis, string period, DateTime fromDate, DateTime toDate, string loadProfile, string putCall,
                              string strike, string samplingPeriod, string priceType, string indexFormula, DateTime transferDate, string certificateType, string swapPriceType,
                              string priority, string expiryTime, int expiryoffset, DateTime expiryDate, DateTime date)
        {
            ExecutionVenue = executionVenue;
            PriceBasis = priceBasis;
            Period = period;
            FromDate = fromDate;
            ToDate = toDate;
            LoadProfile = loadProfile;
            PutCall = putCall;
            Strike = strike;
            SamplingPeriod = samplingPeriod;
            PriceType = priceType;
            IndexFormula = indexFormula;
            TransferDate = transferDate;
            CertificateType = certificateType;
            SwapPriceType = swapPriceType;
            Priority = priority;
            ExpiryTime = expiryTime;
            Expiryoffset = expiryoffset;
            ExpiryDate = expiryDate;
            Date = date;

        }

        public string ExecutionVenue { get; set; }

        public string PriceBasis { get; set; }

        [XmlIgnore]
        public bool PeriodSpecified
        {
            get { return !string.IsNullOrEmpty(this.Period); }
        }

        public string Period { get; set; }

        [XmlIgnore]
        public DateTime FromDate { get; set; }

        [XmlElement(ElementName = "FromDate")]
        public string XmlFromDate
        {
            get { return this.FromDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.FromDate = DateTime.Parse(value); }
        }

        public bool XmlFromDateSpecified { get { return this.FromDate != DateTime.MinValue; } }
        
        [XmlIgnore]
        public DateTime ToDate { get; set; }

        [XmlElement(ElementName = "ToDate")]
        public string XmlToDate
        {
            get { return this.ToDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.ToDate = DateTime.Parse(value); }
        }

        public bool XmlToDateSpecified { get { return this.ToDate != DateTime.MinValue; } }

        [XmlIgnore]
        public DateTime DeliveryDate { get; set; }

        [XmlElement(ElementName = "DeliveryDate")]
        public string XmlDeliveryDate
        {
            get { return this.DeliveryDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.DeliveryDate = DateTime.Parse(value); }
        }

        public bool XmlDeliveryDateSpecified { get { return this.DeliveryDate != DateTime.MinValue; } }

        public string LoadProfile { get; set; }

        public string TimeZone { get; set; }
        
        public string PutCall { get; set; }
   
        public string Strike { get; set; }

        public string SamplingPeriod { get; set; }

        public string PriceType { get; set; }

        public string IndexFormula { get; set; }
      //  public bool TimeZoneSpecified { get { return !string.IsNullOrEmpty(this.TimeZone); } }

        public string BalanceArea { get; set; }

       // public bool BalanceAreaSpecifed { get { return !string.IsNullOrEmpty(this.BalanceArea); } }

        public string BaseCurrency { get; set; }

        public string CrossCurrency { get; set; }

        public bool XmlTransferDateSpecified { get { return this.TransferDate != DateTime.MinValue; } }

        [XmlIgnore]
        public DateTime TransferDate { get; set; }

        [XmlElement(ElementName = "TransferDate")]
        public string XmlTransferDate
        {
            get { return this.TransferDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.TransferDate = DateTime.Parse(value); }
        }

        public string CertificateType { get; set; }

        public string SwapPriceType { get; set; }

        public string Priority { get; set; }

        public string ExpiryTime { get; set; }

        public int Expiryoffset { get; set; }
        
        [XmlIgnore]
        public DateTime ExpiryDate { get; set; }

        [XmlElement(ElementName = "ExpiryDate")]
        public string XmlExpiryDate
        {
            get { return this.ExpiryDate.ToString(Constants.DATETIME_FORMAT); }
            set { this.ExpiryDate = DateTime.Parse(value); }
        }

        public bool XmlExpiryDateSpecified { get { return this.ExpiryDate != DateTime.MinValue; } }

        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement(ElementName = "Date")]
        public string XmlDate
        {
            get { return this.Date.ToString(Constants.DATETIME_FORMAT); }
            set { this.Date = DateTime.Parse(value); }
        }

        public bool XmlDateSpecified { get { return this.Date != DateTime.MinValue; } }
    }
}