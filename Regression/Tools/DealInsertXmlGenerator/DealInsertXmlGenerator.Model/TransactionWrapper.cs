using System;

namespace DealInsertXmlGenerator.Model
{
    public enum TransactionWrapperType
    {
        ElectricityForward,
        ElectricityFuture,
        ElectricityFloating,
        ElectricityAsian,
        ElectricityStructured,
        CurrencyForward,
        OilFuture,
        EmissionFuture,
        GasForward,
        GasSwap,
        OilSwap,
        GasFuture,
        ElectricityEuropean,
        ElCertForward,
        GenCon,
        GasFloatingPrice,
        ElectricityReserveCapacity,
        ElectricitySpot
    }

    public class TransactionWrapper
    {
        public string ExternalId;
        public string Portfolio;
        public string CounterpartyPortfolio;
        public string BuySell;
        public string ExecutionVenue;
        public string PriceBasis;
        public string LoadProfile;
        public DateTime TradeDate;
        public DateTime FromDate;
        public DateTime ToDate;
        public string DeliveryType;
        public double Quantity;
        public double CapacityBidVolume;
        public double CapacityTradeVolume;
        public double CapacityBidPrice;
        public string QuantityUnit;
        public double Price;
        public string Currency;
        public string PriceVolumeUnit;
        public string CurrencySource;
        public string Trader;
        public string PutCall;
        public double Strike;
        public DateTime ExpiryDate;
        public string SamplingPeriod;
        public string PriceType;
        public string IndexFormulaName;
        public double MarketPriceMultiplicator;
        public string Country;
        public string BaseCurrency;
        public string CrossCurrency;
        public DateTime DeliveryDate;
        public string TimeSeriesId;
        public TimeSeriesDetailWrapper[] TimeSeriesDetailWrappers;
        public string TransactionName;
        public string Resolution;
        public string BalanceArea;
        public string UnderlyingInstrumentType;
        public string AuctionType;
        public string HistoricContractPrices;
        public string ModelType;
        public string HistoricMarketPrices;
    }
}