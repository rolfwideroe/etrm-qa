using DealEntryXml.Model;
using DealEntryXmlGenerator.Wrapper.Model;

namespace DealEntryXml.Generator.Model
{
    class DealEntryFactory
    {
        public static DealEntry CreateDealEntryXml(DealEntryWrapper wrapper)
        {

            GeneralData generalData = new GeneralData(wrapper.DeliveryType, wrapper.BuySell, wrapper.TransactionName);

            Portfolios portfolios = new Portfolios(wrapper.Portfolio, wrapper.CounterpartyPortfolio, wrapper.Interconnector);

            InstrumentData instrumentData = new InstrumentData()
            {
                ExecutionVenue = wrapper.ExecutionVenue,
                LoadProfile = wrapper.LoadProfile,
                FromDate = wrapper.FromDate,
                ToDate = wrapper.ToDate,
                Period = wrapper.Period,
                PriceBasis = wrapper.PriceBasis,
                PutCall = wrapper.PutCall,
                SamplingPeriod = wrapper.SamplingPeriod,
                Strike = wrapper.Strike,
                TimeZone = wrapper.TimeZone,
                BalanceArea = wrapper.BalanceArea,
                DeliveryDate = wrapper.DeliveryDate,
                BaseCurrency = wrapper.BaseCurrency,
                CrossCurrency = wrapper.CrossCurrency,
                PriceType = wrapper.PriceType,
                IndexFormula = wrapper.IndexFormula,
                TransferDate = wrapper.TransferDate,
                CertificateType = wrapper.CertificateType,
                SwapPriceType = wrapper.SwapPriceType,
                Priority = wrapper.Priority,
                ExpiryTime = wrapper.ExpiryTime,
                Expiryoffset = wrapper.Expiryoffset,
                ExpiryDate = wrapper.ExpiryDate,
                Date = wrapper.Date,


            };

            SettlementData settlementData = new SettlementData()
            {
                Quantity = wrapper.Quantity,
                QuantityUnit = wrapper.QuantityUnit,
                Currency = wrapper.Currency,
                CurrencySource = wrapper.CurrencySource,
                Price = wrapper.Price,
                PriceMargin = wrapper.PriceMargin,
                MarketPriceMultiplicator = wrapper.MarketPriceMultiplicator,
                PriceVolumeUnit = wrapper.PriceVolumeUnit,
                Resolution =  wrapper.Resolution,
                SettlementRule = wrapper.SettlementRule,
                FlexibleTimeSeries= wrapper.FlexibleTimeSeries
            };

            Fees fees = new Fees()
            {
                Broker = wrapper.Broker,
                BrokerFee = wrapper.BrokerFee,
                BrokerFeeCurrency = wrapper.BrokerFeeCurrency,
                ClearingFee = wrapper.ClearingFee,
                ClearingFeeCurrency = wrapper.ClearingFeeCurrency,
                CommissionFee = wrapper.CommisionFee,
                CommissionFeeCurrency = wrapper.CommisionFeeCurrency,
                TradingFee = wrapper.TradingFee,
                TradingFeeCurrency = wrapper.TradingFeeCurrency
            };

            DealDetails dealDetails = new DealDetails(wrapper.Trader, wrapper.Status, wrapper.TradeDate);

            ReferenceData referenceData = new ReferenceData(wrapper.Comment, wrapper.QuotaRegion);
       
            Transaction transaction = new Transaction()
            {
                Commodity = wrapper.Commodity,
                InstrumentType = wrapper.InstrumentType,
                GeneralData = generalData,
                Portfolios = portfolios,
                InstrumentData = instrumentData,
                SettlementData = settlementData,
                Fees = fees,
                DealDetails = dealDetails,
                ReferenceData = referenceData
            };

            return new DealEntry(transaction);
        }

    }
}
