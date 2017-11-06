using System;
using System.Xml.Serialization;

namespace ElvizTestUtils.QaLookUp
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [Serializable()]
    public class QaTransactionDTO
    {
        public int? TransactionId { get; set; }
        public string DealType { get; set; } // Covers Commodity-InstrumentType
        public QaBuySell? BuySell { get; set; }
        public bool ShouldSerializeBuySell() { return this.BuySell != null; }
        public QaPaidReceived? PaidReceived { get; set; }
        public bool ShouldSerializePaidReceived() { return this.PaidReceived != null; }
        public string ContractModelType { get; set; }
        //  public bool ShouldSerializeContractModelType(){return this.ContractModelType != null;}
        public QaDeliveryType? DeliveryType { get; set; }
        public string DeliveryLocation { get; set; }
        public Portfolios Portfolios { get; set; }
        public InstrumentData InstrumentData { get; set; }
        public SettlementData SettlementData { get; set; }
        public DealDetails DealDetails { get; set; }
        public FeesData FeesData { get; set; }
        public ReferenceData ReferenceData { get; set; }
        public PropertyGroup[] PropertyGroups { get; set; }
        public TransactionWorkFlowDetails TransactionWorkFlowDetails { get; set; }
    }

    public class Portfolios
    {
        public string PortfolioName { get; set; }
        public string PortfolioExternalId { get; set; }
        public string CounterpartyPortfolioName { get; set; }
        public string CounterpartyPortfolioExternalId { get; set; }
    }

    public class InstrumentData
    {
        public string AuctionType { get; set; }
        public string ExecutionVenue { get; set; }
        public string InstrumentName { get; set; }
        public string PriceBasis { get; set; }
        public string PriceBasisToArea { get; set; }
        public DateTime? FromDate { get; set; }
        public bool ShouldSerializeFromDate() { return FromDate != null; }
        public DateTime? ToDate { get; set; }
        public bool ShouldSerializeToDate() { return ToDate != null; }
        public QaPutCall? PutCall { get; set; }
        public bool ShouldSerializePutCall() { return PutCall != null; }
        public double? Strike { get; set; }
        public bool ShouldSerializeStrike() { return Strike != null; }
        public DateTime? ExpiryDate { get; set; }
        public bool ShouldSerializeExpiryDate() { return ExpiryDate != null; }
        public string LoadProfile { get; set; }
        public string TimeZone { get; set; } //replaces  int TimezoneOffset { get; set; }
        public string PriceType { get; set; } // replaces BookPriceType
        public string HistoricContractPrices { get; set; }
        public string ReferencePriceSeries { get; set; }
        public string DestinationReferencePriceSeries { get; set; }
        public string CapFloorPricingResolution { get; set; }
        // public string SwapPriceType { get; set; } // should be deleted
        public string IndexFormula { get; set; }
        public string BaseIsoCurrency { get; set; }
        public string CrossIsoCurrency { get; set; }
        public DateTime? TransferDate { get; set; }
        public bool ShouldSerializeTransferDate() { return TransferDate != null; }
        public string CertificateType { get; set; }
        public string ProductionFacility { get; set; } //Move?
        public string EnvironmentLabel { get; set; } //Move?
        public string FromCountry { get; set; } //Move?
        public string ToCountry { get; set; } //Move?
        public string SamplingPeriod { get; set; }
        public string BalanceArea { get; set; } //Move?
        public string Interconnector { get; set; } // Move?
        public string ExpiryTime { get; set; }
        public int? ExpiryOffset { get; set; }
        public bool ShouldSerializeExpiryOffset() { return ExpiryOffset != null; }
        public string Priority { get; set; }
        public double? MinVol { get; set; }
        public bool ShouldSerializeMinVol() { return MinVol != null; }
        public double? MaxVol { get; set; }
        public bool ShouldSerializeMaxVol() { return MaxVol != null; }
        public string CapacityId { get; set; }
        public string UnderlyingExecutionVenue { get; set; }
        public string UnderlyingDeliveryType { get; set; }
        public string UnderlyingInstrumentType { get; set; }
        public bool ShouldSerializeSamplingFrom() { return SamplingFrom != null; }
        public DateTime? SamplingFrom { get; set; }
        public bool ShouldSerializeSamplingTo() { return SamplingTo != null; }
        public DateTime? SamplingTo { get; set; }
        public string VolumeReferenceExternalId { get; set; }
        public double? LossFactor { get; set; }

    }

    public class SettlementData
    {
        public double? CapacityBidQuantity { get; set; }
        public double? CapacityTradedQuantity { get; set; }
        public bool ShouldSerializeCapacityBidQuantity() { return CapacityBidQuantity != null; }
        public bool ShouldSerializeCapacityTradedQuantity() { return CapacityTradedQuantity != null; }
        public double? CapacityPrice { get; set; }
        public bool ShouldSerializeCapacityPrice() { return CapacityPrice != null; }
        public double? Quantity { get; set; }
        public bool ShouldSerializeQuantity() { return Quantity != null; }
        public string QuantityUnit { get; set; }
        public double? Price { get; set; }
        public bool ShouldSerializePrice() { return Price != null; }
        public double? InitialPrice { get; set; }
        public bool ShouldSerializeInitialPrice() { return InitialPrice != null; }
        public string PriceIsoCurrency { get; set; }
        public string PriceVolumeUnit { get; set; }
        public string CurrencySource { get; set; }
        public double? MarketPriceMultiplicator { get; set; }
        public bool ShouldSerializeMarketPriceMultiplicator() { return MarketPriceMultiplicator != null; }
        public string MasterAgreement { get; set; }
        public string SettlementRule { get; set; }
        public string Resolution { get; set; }
        public TimeSeriesSet TimeSeriesSet { get; set; }
        public PriceVolumeTimeSeriesDetail[] PriceVolumeTimeSeriesDetails { get; set; }
        public QaVppNomination[] NominationsHourly { get; set; }
    }

    public class DealDetails
    {
        public DealDetails()
        {
        }

        public string Trader { get; set; }
        public string CounterpartyTrader { get; set; }
        public string Status { get; set; }
        public DateTime? TradeDateTimeUtc { get; set; }

    }

    public class QaVppNomination
    {
        public QaVppNomination()
        {
        }

        public DateTime FromTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public double Value { get; set; }
    }

    public class ReferenceData
    {
        public ReferenceData()
        {
        }



        public string ExternalId { get; set; }
        public string ExternalSource { get; set; }
        public string TicketNumber { get; set; }
        //   [XmlElement(IsNullable = true)]
        public DealGroup[] DealGroups { get; set; }
        public string Comment { get; set; }
        public DateTime ModificationDateTimeUtc { get; set; }
        //  public bool ShouldSerializeModificationDate() { return ModificationDateTimeUtc != null; }
        public string QuotaRegion { get; set; }
        public double RiskValue { get; set; }
        public bool Originator { get; set; }
        public int Deliveries { get; set; }
        public int? DeclareId { get; set; }
        public bool ShouldSerializeDeclareId() { return DeclareId != null; }
        public int? ContractSplitId { get; set; }
        public bool ShouldSerializeContractSplitId() { return ContractSplitId != null; }
        public int? DistributionParentTransactionId { get; set; }
        public bool ShouldSerializeDistributionParentTransactionId() { return DistributionParentTransactionId != null; }
        public double? DistributedQuantity { get; set; }
        public bool ShouldSerializeDistributedQuantity() { return DistributedQuantity != null; }
        public double OriginalQuantity { get; set; }
        public int ReferringId { get; set; }
        public int[] CascadingOriginIds { get; set; }

    }


    public class TimeSeriesSet
    {
        public TimeSeries[] TimeSeries { get; set; }
    }


    public class TimeSeries
    {
        public string Resolution { get; set; }

        public string TimeSeriesTypeName { get; set; }

        public string TimezoneName { get; set; }

        public TimeSeriesValue[] TimeSeriesValues { get; set; }
    }

    public class TimeSeriesValue
    {
        [XmlAttribute]
        public DateTime FromDateTime { get; set; }

        [XmlAttribute]
        public DateTime ToDateTime { get; set; }

        //[XmlAttribute]
        //public DateTime UtcFromDateTime { get; set; }

        //[XmlAttribute]
        //public DateTime UtcToDateTime { get; set; }

        [XmlAttribute("Value")]
        public string XmlValue { get; set; }
        //public bool ShouldSerializeValue() { return Value != null; }

        [XmlIgnore]
        public double? Value
        {
            get
            {
                if (string.IsNullOrEmpty(this.XmlValue))
                {
                    return null;
                }
                return double.Parse(this.XmlValue);
            }
            set
            {
                if (value != null)
                {
                    this.XmlValue = value.ToString();
                }

            }
        }
    }



    public class FeesData
    {
        public FeesData()
        {
        }

        public string Broker { get; set; }
        public string ClearingType { get; set; }
        public Fee[] Fees { get; set; }
    }

    public class Fee // Broker, Clearing, Trading, Entry, Exit, Nomination
    {
        public Fee()
        {
        }

        public Fee(string feeType, double feeValue, string feeUnit, string feeValueType)
        {
            FeeType = feeType;
            FeeValue = feeValue;
            FeeUnit = feeUnit;
            FeeValueType = feeValueType;
        }
        [XmlAttribute]
        public string FeeType { get; set; }
        [XmlAttribute]
        public string FeeValueType { get; set; }
        [XmlAttribute]
        public double FeeValue { get; set; }
        [XmlAttribute]
        public string FeeUnit { get; set; }
    }

    public class DealGroup
    {
        public DealGroup()
        {
        }

        public DealGroup(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }


    public enum QaDeliveryType
    {
        Financial,
        Physical
    }

    public enum QaPutCall
    {
        Put,
        Call
    }


    public class Property
    {
        public Property()
        {
        }

        public Property(string name, string value, string valueType)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
        }

        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Value;
        [XmlAttribute]
        public string ValueType;

    }



    public class PropertyGroup
    {
        public PropertyGroup()
        {
        }

        [XmlAttribute]
        public string Name;
        public Property[] Properties;
    }

    public enum QaBuySell
    {
        Buy,
        Sell
    }

    public enum QaPaidReceived
    {
        Paid,
        Received
    }

    public enum QaPropertyType
    {
        StringPropery,
        DateProperty,
        BooleanProperty,
        DoublePropery,
    }

    public class TransactionWorkFlowDetails
    {
        public TransactionWorkFlowDetails()
        {
        }

        public bool Authorised { get; set; }
        public bool Paid { get; set; }
        public bool ConfirmedByBroker { get; set; }
        public bool ConfirmedByCounterparty { get; set; }
        public bool Audited { get; set; }
        public DateTime? TimeStampAuthorised { get; set; }
        public bool ShouldSerializeTimeStampAuthorised() { return TimeStampAuthorised != null; }
        public DateTime? TimeStampCounterpartyAuthorised { get; set; }
        public bool ShouldSerializeTimeStampCounterpartyAuthorised() { return TimeStampCounterpartyAuthorised != null; }
        public DateTime? TimeStampClearedUtc { get; set; }
        public bool ShouldSerializeTimeStampClearedUtc() { return TimeStampClearedUtc != null; }
        public DateTime? TimeStampConfirmationBrokerUtc { get; set; }
        public bool ShouldSerializeTimeStampConfirmationBrokerUtc() { return TimeStampConfirmationBrokerUtc != null; }
        public DateTime? TimeStampConfirmationCounterPartyUtc { get; set; }
        public bool ShouldSerializeTimeStampConfirmationCounterPartyUtc() { return TimeStampConfirmationCounterPartyUtc != null; }
    }

    public class PriceVolumeTimeSeriesDetail
    {
        public PriceVolumeTimeSeriesDetail()
        {
        }
        [XmlAttribute]
        public DateTime FromDateTime { get; set; }
        [XmlAttribute]
        public DateTime ToDateTime { get; set; }
        [XmlAttribute]
        public double Price { get; set; }
        [XmlAttribute]
        public double Volume { get; set; }
    }

}
// CashFlowTransactionType CashFlowType { get; set; }









// string UserSpecifiedName { get; set; } // Duplicate of InstrumentName






// string EnergySource { get; set; }









//double ThresholdPrice { get; set; }

