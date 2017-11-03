using System;

namespace DealInsertExtended
{
    public class DealInsertFactory
    {
        private const string Trader = "DealImport";
        private const string PriceSource = "RegTest";


        public static DealInsertMetaData CreateDefaultDealInsertMetaData()
        {
            return new DealInsertMetaData()
                       {
                           TimezoneSpecified = true,
                           Timezone = TimezoneType.CET
                       };
        }


        private static DealInsertTransactionReferenceData DefaultTransactionReferenceData(string externalId)
        {
            DealInsertTransactionReferenceData referenceData = new DealInsertTransactionReferenceData
                                                                   {
                                                                       ExternalId = externalId,
                                                                       ExternalSource = PriceSource
                                                                   };
            return referenceData;
        }

        private static DealInsertTransactionDealDetails DefaultTransactionDealDetails(DateTime tradeDate)
        {
            DealInsertTransactionDealDetails details = new DealInsertTransactionDealDetails()
                                                           {
                                                               AuditedSpecified = false,
                                                               AuthorisedSpecified = false,
                                                               ConfirmedByBrokerSpecified = false,
                                                               ConfirmedByCounterpartySpecified = false,
                                                               PaidSpecified = false,
                                                               Status = StatusType.Active,
                                                               TradeDate = tradeDate,
                                                               Trader = Trader
                                                           };
            return details;
        }


        public static DealInsertTransaction CreateCurrencyForward(string porfolio,
                                                                  string couterpartyPorfolio,
                                                                  string buySell,
                                                                  string baseCurrency,
                                                                  string crossCurrency,
                                                                  DateTime deliverydate,
                                                                  double price,
                                                                  double quantity,
                                                                  string currencySource,
                                                                  DateTime tradeDate,
                                                                  string externalId

            )
        {
            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);

            CurrencyForwardType currencyForward = new CurrencyForwardType
                                                      {
                                                          InstrumentData =
                                                              new CurrencyForwardTypeInstrumentData()
                                                                  {
                                                                      TimezoneSpecified = true,
                                                                      ExecutionVenue = ExecutionVenueType.None,
                                                                      Timezone = TimezoneType.CET,
                                                                      BaseCurrency =
                                                                          DealInsertParser.GetCurrencyType(baseCurrency),
                                                                      CrossCurrency =
                                                                          DealInsertParser.GetCurrencyType(crossCurrency),
                                                                      DeliveryDate = deliverydate
                                                                  },
                                                          SettlementData =
                                                              new CurrencyForwardTypeSettlementData()
                                                                  {
                                                                      Price = price,
                                                                      Quantity = quantity,
                                                                      CurrencySource =
                                                                          DealInsertParser.GetCurrencySourceType(
                                                                              currencySource)

                                                                  }
                                                      };


            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, currencyForward,
                                                                DealInsertTransactionDealType.CurrencyForward);
        }

        public static DealInsertTransaction CreateElectricityFuture(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea
            )
        {
            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);

            DealInsertTransactionElectricityFuture future = new DealInsertTransactionElectricityFuture
                                                                {
                                                                    InstrumentData =
                                                                        new DealInsertTransactionElectricityFutureInstrumentData
                                                                        ()
                                                                            {
                                                                                ExecutionVenue =
                                                                                    DealInsertParser.GetExecutionVenue(
                                                                                        executionVenue),
                                                                                FromDate = fromDate,
                                                                                ToDate = toDate,
                                                                                Item =
                                                                                    DealInsertParser.
                                                                                    GetFinancialPhysical(
                                                                                        financialPhysical,balanceArea),
                                                                                LoadProfile = loadProfile,
                                                                                PriceBasis = priceBasis,
                                                                                TimezoneSpecified = false
                                                                            },
                                                                    SettlementData =
                                                                        new DealInsertTransactionElectricityFutureSettlementData
                                                                        ()
                                                                            {

                                                                                Price = price,
                                                                                Quantity = quantity,
                                                                                Currency =
                                                                                    DealInsertParser.GetCurrencyType(
                                                                                        currency),
                                                                                CurrencySource =
                                                                                    DealInsertParser.
                                                                                    GetCurrencySourceType(currencySource),
                                                                                Clearing = ClearingType.Electronic,
                                                                                ClearingCurrency = CurrencyType.EUR,
                                                                                BrokerCurrency = CurrencyType.EUR


                                                                            }

                                                                };


            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, future,
                                                                DealInsertTransactionDealType.ElectricityFuture);
        }



        public static DealInsertTransaction CreateElectricityForward(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionElectricityForward forward = new DealInsertTransactionElectricityForward
                                                                  {
                                                                      InstrumentData =
                                                                          new DealInsertTransactionElectricityForwardInstrumentData
                                                                          ()
                                                                              {
                                                                                  ExecutionVenue =
                                                                                      DealInsertParser.GetExecutionVenue
                                                                                      (executionVenue),
                                                                                  FromDate = fromDate,
                                                                                  ToDate = toDate,
                                                                                  LoadProfile = loadProfile,
                                                                                  PriceBasis = priceBasis,
                                                                                  TimezoneSpecified = false,
                                                                                  Item =
                                                                                      DealInsertParser.
                                                                                      GetFinancialPhysical(
                                                                                          financialPhysical,balanceArea)

                                                                              },
                                                                      SettlementData =
                                                                          new DealInsertTransactionElectricityForwardSettlementData
                                                                          ()
                                                                              {

                                                                                  Price = price,
                                                                                  Quantity = quantity,
                                                                                  Currency =
                                                                                      DealInsertParser.GetCurrencyType(
                                                                                          currency),
                                                                                  CurrencySource =
                                                                                      DealInsertParser.
                                                                                      GetCurrencySourceType(
                                                                                          currencySource),
                                                                                  Clearing = ClearingType.Electronic,
                                                                                  ClearingCurrency = CurrencyType.EUR,
                                                                                  BrokerCurrency = CurrencyType.EUR


                                                                              }


                                                                  };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, forward,
                                                                DealInsertTransactionDealType.ElectricityForward);
        }

        public static DealInsertTransaction CreateElectricityEuropean(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            DateTime expiryDate,
            string financialPhysical,
            string putCall,
            double price,
            double strike,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea,
            string underlyingInstrumentType,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            string resolution
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);


            object item =  quantity;
            if (!string.IsNullOrEmpty(resolution))
                item = DealInsertHelper.GetElectricityEuropeanTimeSeriesDetail(resolution, fromDates, toDates, volumes);

            DealInsertTransactionElectricityEuropean forward = new DealInsertTransactionElectricityEuropean
                                                                   {
                                                                       InstrumentData =
                                                                           new DealInsertTransactionElectricityEuropeanInstrumentData
                                                                           ()
                                                                               {
                                                                                   ExecutionVenue =
                                                                                       DealInsertParser.
                                                                                       GetExecutionVenue
                                                                                       (executionVenue),
                                                                                   FromDate = fromDate,
                                                                                   ToDate = toDate,
                                                                                   LoadProfile = loadProfile,
                                                                                   PriceBasis = priceBasis,
                                                                                   TimezoneSpecified = false,
                                                                                   Item =
                                                                                       DealInsertParser.
                                                                                       GetFinancialPhysical(
                                                                                           financialPhysical,balanceArea)
                                                                                   ,
                                                                                   PutCall =
                                                                                       DealInsertParser.GetPutCallType(
                                                                                           putCall),
                                                                                   Strike = strike,
                                                                                   ExpiryDate = expiryDate,
                                                                                   UnderlyingInstrumentType = underlyingInstrumentType
                                                                               },
                                                                       SettlementData =
                                                                           new DealInsertTransactionElectricityEuropeanSettlementData
                                                                           ()
                                                                               {

                                                                                   Price = price,
                                                                                   Item = item,
                                                                                   Currency =
                                                                                       DealInsertParser.GetCurrencyType(
                                                                                           currency),
                                                                                   CurrencySource =
                                                                                       DealInsertParser.
                                                                                       GetCurrencySourceType(
                                                                                           currencySource),
                                                                                   Clearing = ClearingType.Electronic,
                                                                                   ClearingCurrency = CurrencyType.EUR,
                                                                                   BrokerCurrency = CurrencyType.EUR


                                                                               }


                                                                   };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);

            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, forward,
                                                                DealInsertTransactionDealType.ElectricityEuropean);
        }

        public static DealInsertTransaction CreateElectricityFloatingPrice(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            string priceType,
            string indexFormulaName,
            double marketpriceMultiplikator,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionElectricityFloatingPrice floating = new DealInsertTransactionElectricityFloatingPrice
                                                                         {
                                                                             InstrumentData =
                                                                                 new DealInsertTransactionElectricityFloatingPriceInstrumentData
                                                                                 ()
                                                                                     {
                                                                                         ExecutionVenue =
                                                                                             DealInsertParser.
                                                                                             GetExecutionVenue
                                                                                             (executionVenue),
                                                                                         FromDate = fromDate,
                                                                                         ToDate = toDate,
                                                                                         LoadProfile = loadProfile,
                                                                                         PriceBasis = priceBasis,
                                                                                         TimezoneSpecified = false,
                                                                                         Item =
                                                                                             DealInsertParser.
                                                                                             GetFinancialPhysical(
                                                                                                 financialPhysical,balanceArea),
                                                                                         PriceType =
                                                                                             DealInsertParser.
                                                                                             GetElFloatingPriceType(
                                                                                                 priceType),
                                                                                         IndexFormulaName =
                                                                                             indexFormulaName,
                                                                                         PriceTypeSpecified = true


                                                                                     },
                                                                             SettlementData =
                                                                                 new DealInsertTransactionElectricityFloatingPriceSettlementData
                                                                                 ()
                                                                                     {

                                                                                         PriceMargin = price,
                                                                                         MarketPriceMultiplicator =
                                                                                             marketpriceMultiplikator,
                                                                                         Quantity = quantity,
                                                                                         Currency =
                                                                                             DealInsertParser.
                                                                                             GetCurrencyType(currency),
                                                                                         CurrencySource =
                                                                                             DealInsertParser.
                                                                                             GetCurrencySourceType(
                                                                                                 currencySource),
                                                                                         Clearing =
                                                                                             ClearingType.Electronic,
                                                                                         ClearingCurrency =
                                                                                             CurrencyType.EUR,
                                                                                         BrokerCurrency =
                                                                                             CurrencyType.EUR


                                                                                     }


                                                                         };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, floating,
                                                                DealInsertTransactionDealType.ElectricityFloatingPrice);
        }

        public static DealInsertTransaction CreateElectricityAsian(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            string putCall,
            double strike,
            string samplingPeriod,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            string resolution
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);



            object item = quantity;
            if (!string.IsNullOrEmpty(resolution))
                item = DealInsertHelper.GetElectricityAsianTimeSeriesDetail(resolution, fromDates, toDates, volumes);

            DealInsertTransactionElectricityAsian asian = new DealInsertTransactionElectricityAsian
                                                              {
                                                                  InstrumentData =
                                                                      new DealInsertTransactionElectricityAsianInstrumentData
                                                                      ()
                                                                          {
                                                                              ExecutionVenue =
                                                                                  DealInsertParser.GetExecutionVenue
                                                                                  (executionVenue),
                                                                              FromDate = fromDate,
                                                                              ToDate = toDate,
                                                                              LoadProfile = loadProfile,
                                                                              PriceBasis = priceBasis,
                                                                              TimezoneSpecified = false,
                                                                              Financial = new FinancialType(),
                                                                              PutCall =
                                                                                  DealInsertParser.GetPutCallType(
                                                                                      putCall),
                                                                              SamplingPeriod =
                                                                                  DealInsertParser.GetSamplingPeriodType
                                                                                  (samplingPeriod),
                                                                              Strike = strike


                                                                          },
                                                                  SettlementData =
                                                                      new DealInsertTransactionElectricityAsianSettlementData
                                                                      ()
                                                                          {

                                                                              Price = price,
                                                                              Item = item,
                                                                              Currency =
                                                                                  DealInsertParser.GetCurrencyType(
                                                                                      currency),
                                                                              CurrencySource =
                                                                                  DealInsertParser.GetCurrencySourceType
                                                                                  (currencySource),
                                                                              Clearing = ClearingType.Electronic
                                                                            


                                                                          }


                                                              };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, asian,
                                                                DealInsertTransactionDealType.ElectricityAsian);
        }

        public static DealInsertTransaction CreateOilFuture(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            string priceType,
            double price,
            double quantity,
            string quantityType,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionOilFuture oilFuture = new DealInsertTransactionOilFuture
                                                           {
                                                               InstrumentData =
                                                                   new DealInsertTransactionOilFutureInstrumentData()
                                                                       {
                                                                           ExecutionVenue =
                                                                               DealInsertParser.GetExecutionVenue
                                                                               (executionVenue),
                                                                           FromDate = fromDate,
                                                                           ToDate = toDate,
                                                                           PriceBasis = priceBasis,
                                                                           TimezoneSpecified = false,
                                                                           Item =
                                                                               DealInsertParser.GetFinancialPhysical(
                                                                                   financialPhysical,"")



                                                                       },
                                                               SettlementData =
                                                                   new DealInsertTransactionOilFutureSettlementData()
                                                                       {

                                                                           Price = price,
                                                                           PriceVolumeUnitSpecified = false,
                                                                           QuantityUnit =
                                                                               DealInsertParser.GetQuantityUnitTypeOil(
                                                                                   quantityType),
                                                                           Quantity = quantity,
                                                                           Currency =
                                                                               DealInsertParser.GetCurrencyType(currency),
                                                                           CurrencySource =
                                                                               DealInsertParser.GetCurrencySourceType(
                                                                                   currencySource),
                                                                           Clearing = ClearingType.Electronic,
                                                                           ClearingCurrency = CurrencyType.USD,
                                                                           BrokerCurrency = CurrencyType.USD,


                                                                       }


                                                           };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, oilFuture,
                                                                DealInsertTransactionDealType.OilFuture);
        }

        public static DealInsertTransaction CreateElectricityStructuredDealFixed(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            string loadProfile,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string transactionName,
            string resolution,
            string priceType,
            string historicContractPrices,
            string historicMarketPrices,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            double[] prices,
            double[] volumesOutsideProfile,
            double[] pricesOutsideProfile,
            string balanceArea
            )
        {
            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);

            PriceSeriesType contractPriceSeriesType = !string.IsNullOrEmpty(historicContractPrices) ? new PriceSeriesType { ExternalId = historicContractPrices } : null;
            PriceSeriesType marketPriceSeriesType = !string.IsNullOrEmpty(historicMarketPrices) ? new PriceSeriesType { ExternalId = historicMarketPrices } : null;
            DealInsertTransactionElectricityStructuredDeal structuredDeal = new DealInsertTransactionElectricityStructuredDeal
            {
                InstrumentData =
                    new DealInsertTransactionElectricityStructuredDealInstrumentData
                        ()
                    {
                        ExecutionVenue = DealInsertParser.GetExecutionVenue(executionVenue),
                        FromDate = fromDate,
                        ToDate = toDate,
                        LoadProfile = loadProfile,
                        PriceBasis = priceBasis,
                        TimezoneSpecified = false,
                        Item = DealInsertParser.GetFinancialPhysical(financialPhysical, balanceArea),
                        PriceType = DealInsertParser.GetPriceType(priceType),
                        TransactionName = transactionName,
                        HistoricContractPrices = contractPriceSeriesType,
                        HistoricMarketPrices = marketPriceSeriesType

                    },
                SettlementData =
                    new DealInsertTransactionElectricityStructuredDealSettlementData
                        ()
                    {
                        Currency = DealInsertParser.GetCurrencyType(currency),
                        CurrencySource = DealInsertParser.GetCurrencySourceType(currencySource),
                        Clearing = ClearingType.Electronic,
                        ClearingCurrency = DealInsertParser.GetCurrencyType(currency),
                        BrokerCurrency = DealInsertParser.GetCurrencyType(currency),
                        PriceVolumeUnitSpecified = false,
                        Item = DealInsertHelper.GetElectricityStructuredDealInformation
                                   (loadProfile,
                                    resolution,
                                    fromDates, 
                                    toDates,
                                    volumes, 
                                    prices,
                                    volumesOutsideProfile,
                                    pricesOutsideProfile)
                    }
            };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);

            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, structuredDeal,
                                                                DealInsertTransactionDealType.ElectricityStructuredDeal);
        }

        public static DealInsertTransaction CreateElectricityStructuredDealFlex(string porfolio, string couterpartyPorfolio, string buySell, string executionVenue,
            string priceBasis, string loadProfile, DateTime fromDate, DateTime toDate, string financialPhysical, string currency, string currencySource, DateTime tradeDate,
            string externalId, string transactionName, string resolution, string priceType, string historicContractPrices, string historicMarketPrices, DateTime[] fromDates,
            DateTime[] toDates, double[] volumes, double[] prices, double[] realizedVolumes, double[] realizedPrices, string balanceArea)
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell, porfolio, couterpartyPorfolio);

            PriceSeriesType contractPriceSeriesType = !string.IsNullOrEmpty(historicContractPrices) ? new PriceSeriesType {ExternalId = historicContractPrices} : null;
            PriceSeriesType marketPriceSeriesType = !string.IsNullOrEmpty(historicMarketPrices) ? new PriceSeriesType {ExternalId = historicMarketPrices} : null;
            DealInsertTransactionElectricityStructuredDeal structuredDeal = new DealInsertTransactionElectricityStructuredDeal
            {
                InstrumentData =
                    new DealInsertTransactionElectricityStructuredDealInstrumentData
                        ()
                    {
                        ExecutionVenue = DealInsertParser.GetExecutionVenue(executionVenue),
                        FromDate = fromDate,
                        ToDate = toDate,
                        LoadProfile = loadProfile,
                        PriceBasis = priceBasis,
                        TimezoneSpecified = false,
                        Item = DealInsertParser.GetFinancialPhysical(financialPhysical, balanceArea),
                        PriceType = DealInsertParser.GetPriceType(priceType),
                        TransactionName = transactionName,
                        HistoricContractPrices = contractPriceSeriesType,
                        HistoricMarketPrices = marketPriceSeriesType

                    },
                SettlementData =
                    new DealInsertTransactionElectricityStructuredDealSettlementData
                        ()
                    {
                        Currency = DealInsertParser.GetCurrencyType(currency),
                        CurrencySource = DealInsertParser.GetCurrencySourceType(currencySource),
                        Clearing = ClearingType.Electronic,
                        ClearingCurrency = DealInsertParser.GetCurrencyType(currency),
                        BrokerCurrency = DealInsertParser.GetCurrencyType(currency),
                        PriceVolumeUnitSpecified = false,
                        Item = DealInsertHelper.GetElectricityStructuredDealFlexInformation
                            (loadProfile,
                                resolution,
                                fromDates,
                                toDates,
                                volumes,
                                prices,
                                realizedVolumes,
                                realizedPrices)
                    }
            };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);

            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, structuredDeal, DealInsertTransactionDealType.ElectricityStructuredDeal);
        }

        public static DealInsertTransaction CreateEmissionFuture(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            DateTime settlementDate,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionEmissionFuture emissionFuture = new DealInsertTransactionEmissionFuture
                                                                     {
                                                                         InstrumentData =
                                                                             new DealInsertTransactionEmissionFutureInstrumentData
                                                                             ()
                                                                                 {
                                                                                     ExecutionVenue =
                                                                                         DealInsertParser.
                                                                                         GetExecutionVenue(
                                                                                             executionVenue),
                                                                                     SettlementDate = settlementDate,
                                                                                     PriceBasis = priceBasis,
                                                                                     TimezoneSpecified = false,


                                                                                 },
                                                                         SettlementData =
                                                                             new DealInsertTransactionEmissionFutureSettlementData
                                                                             ()
                                                                                 {

                                                                                     Price = price,
                                                                                     Quantity = quantity,
                                                                                     Currency =
                                                                                         DealInsertParser.
                                                                                         GetCurrencyType(currency),
                                                                                     CurrencySource =
                                                                                         DealInsertParser.
                                                                                         GetCurrencySourceType(
                                                                                             currencySource),
                                                                                     Clearing = ClearingType.Electronic,
                                                                                     ClearingCurrency = CurrencyType.EUR,
                                                                                     BrokerCurrency = CurrencyType.EUR,
                                                                                 }


                                                                     };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, emissionFuture,
                                                                DealInsertTransactionDealType.EmissionFuture);
        }

        public static DealInsertTransaction CreateGasForward(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            double price,
            string priceUnit,
            double quantity,
            string quanityUnit,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionGasForward forward = new DealInsertTransactionGasForward
                                                          {
                                                              InstrumentData =
                                                                  new DealInsertTransactionGasForwardInstrumentData()
                                                                      {
                                                                          ExecutionVenue =
                                                                              DealInsertParser.GetExecutionVenue(
                                                                                  executionVenue),
                                                                          FromDate = fromDate,
                                                                          ToDate = toDate,
                                                                          LoadProfile = "Base",
                                                                          PriceBasis = priceBasis,
                                                                          TimezoneSpecified = false,
                                                                          Item =
                                                                              DealInsertParser.GetFinancialPhysical(
                                                                                  financialPhysical,balanceArea)

                                                                      },
                                                              SettlementData =
                                                                  new DealInsertTransactionGasForwardSettlementData
                                                                      {
                                                                          Price = price,
                                                                          Quantity = quantity,
                                                                          Currency =
                                                                              DealInsertParser.GetCurrencyType(currency),
                                                                          CurrencySource =
                                                                              DealInsertParser.GetCurrencySourceType(
                                                                                  currencySource),
                                                                          Clearing = ClearingType.Electronic,
                                                                          ClearingCurrency = CurrencyType.EUR,
                                                                          BrokerCurrency = CurrencyType.EUR,
                                                                          QuantityUnit =
                                                                              DealInsertParser.GetGasQuantityUnit(
                                                                                  quanityUnit),
                                                                          PriceVolumeUnitSpecified = true,
                                                                          PriceVolumeUnit =
                                                                              DealInsertParser.GetGasPriceUnit(priceUnit),
                                                                      }


                                                          };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, forward,
                                                                DealInsertTransactionDealType.GasForward);
        }

        public static DealInsertTransaction CreateGasSwap(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string indexFormula,
            DateTime fromDate,
            DateTime toDate,
            string priceUnit,
            string quanityUnit,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            double[] prices,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionGasSwap fixedForFloating = new DealInsertTransactionGasSwap()
                                                                {
                                                                    InstrumentData =
                                                                        new DealInsertTransactionGasSwapInstrumentData()
                                                                            {
                                                                                ExecutionVenue =
                                                                                    DealInsertParser.GetExecutionVenue(
                                                                                        executionVenue),
                                                                                FromDate = fromDate,
                                                                                ToDate = toDate,
                                                                                PriceType =
                                                                                    PriceTypeGasSwap.FixedForFloating,
                                                                                TimezoneSpecified = false,
                                                                                IndexFormulaName = indexFormula


                                                                            },
                                                                    SettlementData =
                                                                        new DealInsertTransactionGasSwapSettlementData()
                                                                            {
                                                                                Currency =
                                                                                    DealInsertParser.GetCurrencyType(
                                                                                        currency),
                                                                                CurrencySource =
                                                                                    DealInsertParser.
                                                                                    GetCurrencySourceType(currencySource),
                                                                                Clearing = ClearingType.Electronic,
                                                                                ClearingCurrency = CurrencyType.EUR,
                                                                                BrokerCurrency = CurrencyType.EUR,
                                                                                QuantityUnit =
                                                                                    DealInsertParser.GetGasQuantityUnit(
                                                                                        quanityUnit),
                                                                                PriceVolumeUnitSpecified = true,
                                                                                PriceVolumeUnit =
                                                                                    DealInsertParser.
                                                                                    GetGasPriceVolumeUnit(priceUnit),
                                                                                PriceVolumeTimeSeries =
                                                                                    DealInsertHelper.
                                                                                    GetGasSwapTimeSeries(fromDates,
                                                                                                         toDates,
                                                                                                         volumes, prices)

                                                                            }


                                                                };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, fixedForFloating,
                                                                DealInsertTransactionDealType.GasSwap);
        }

        public static DealInsertTransaction CreateOilSwap(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string indexFormula,
            DateTime fromDate,
            DateTime toDate,
            string priceUnit,
            string quanityUnit,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            double[] prices,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionOilSwap fixedForFloating = new DealInsertTransactionOilSwap()
                                                                {
                                                                    InstrumentData =
                                                                        new DealInsertTransactionOilSwapInstrumentData()
                                                                            {
                                                                                ExecutionVenue =
                                                                                    DealInsertParser.GetExecutionVenue(
                                                                                        executionVenue),
                                                                                FromDate = fromDate,
                                                                                ToDate = toDate,
                                                                                PriceType =
                                                                                    PriceTypeOilSwap.FixedForFloating,
                                                                                TimezoneSpecified = false,
                                                                                IndexFormulaName = indexFormula


                                                                            },
                                                                    SettlementData =
                                                                        new DealInsertTransactionOilSwapSettlementData()
                                                                            {
                                                                                Currency =
                                                                                    DealInsertParser.GetCurrencyType(
                                                                                        currency),
                                                                                CurrencySource =
                                                                                    DealInsertParser.
                                                                                    GetCurrencySourceType(currencySource),
                                                                                Clearing = ClearingType.Electronic,
                                                                                ClearingCurrency = CurrencyType.USD,
                                                                                BrokerCurrency = CurrencyType.USD,
                                                                                QuantityUnit =
                                                                                    DealInsertParser.
                                                                                    GetQuantityUnitTypeOil(quanityUnit),
                                                                                PriceVolumeUnitSpecified = true,
                                                                                PriceVolumeUnit =
                                                                                    DealInsertParser.
                                                                                    GetPriceVolumeUnitTypeOil(priceUnit),
                                                                                PriceVolumeTimeSeries =
                                                                                    DealInsertHelper.
                                                                                    GetOilSwapTimeSeries(fromDates,
                                                                                                         toDates,
                                                                                                         volumes, prices)

                                                                            }


                                                                };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, fixedForFloating,
                                                                DealInsertTransactionDealType.OilSwap);
        }


        public static DealInsertTransaction CreateGasFuture(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            double price,
            string priceUnit,
            double quantity,
            string quanityUnit,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string balanceArea
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionGasFuture future = new DealInsertTransactionGasFuture
                                                        {
                                                            InstrumentData =
                                                                new DealInsertTransactionGasFutureInstrumentData()
                                                                    {
                                                                        ExecutionVenue =
                                                                            DealInsertParser.GetExecutionVenue(
                                                                                executionVenue),
                                                                        FromDate = fromDate,
                                                                        ToDate = toDate,
                                                                        LoadProfile = "Base",
                                                                        PriceBasis = priceBasis,
                                                                        TimezoneSpecified = false,
                                                                        Item =
                                                                            DealInsertParser.GetFinancialPhysical(
                                                                                financialPhysical,balanceArea)

                                                                    },
                                                            SettlementData =
                                                                new DealInsertTransactionGasFutureSettlementData()
                                                                    {
                                                                        Price = price,
                                                                        Quantity = quantity,
                                                                        Currency =
                                                                            DealInsertParser.GetCurrencyType(currency),
                                                                        CurrencySource =
                                                                            DealInsertParser.GetCurrencySourceType(
                                                                                currencySource),
                                                                        Clearing = ClearingType.Electronic,
                                                                        ClearingCurrency = CurrencyType.EUR,
                                                                        BrokerCurrency = CurrencyType.EUR,
                                                                        QuantityUnit =
                                                                            DealInsertParser.GetGasFutureQuantityUnit(
                                                                                quanityUnit),
                                                                        PriceVolumeUnitSpecified = true,
                                                                        PriceVolumeUnit =
                                                                            DealInsertParser.GetGasVolumeUnitRestricted(
                                                                                priceUnit)
                                                                    }


                                                        };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, future,
                                                                DealInsertTransactionDealType.GasFuture);
        }

        public static DealInsertTransaction CreateElCertForward(
            string porfolio,
            string couterpartyPorfolio,
            string buySell,
            string executionVenue,
            string priceBasis,
            DateTime deliveryDate,
            double price,
            double quantity,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId
            )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionElcertificateForward forward = new DealInsertTransactionElcertificateForward
                                                                    {
                                                                        InstrumentData =
                                                                            new DealInsertTransactionElcertificateForwardInstrumentData
                                                                            ()
                                                                                {
                                                                                    ExecutionVenue =
                                                                                        DealInsertParser.
                                                                                        GetExecutionVenue(executionVenue),
                                                                                    SettlementDate = deliveryDate,
                                                                                    PriceBasis = priceBasis,
                                                                                    TimezoneSpecified = false,
                                                                                    Item = new PhysicalType()

                                                                                },
                                                                        SettlementData =
                                                                            new DealInsertTransactionElcertificateForwardSettlementData
                                                                            ()
                                                                                {
                                                                                    Price = price,
                                                                                    Quantity = quantity,
                                                                                    Currency =
                                                                                        DealInsertParser.GetCurrencyType
                                                                                        (currency),
                                                                                    CurrencySource =
                                                                                        DealInsertParser.
                                                                                        GetCurrencySourceType(
                                                                                            currencySource),
                                                                                    Clearing = ClearingType.Electronic,
                                                                                    ClearingCurrency = CurrencyType.EUR,
                                                                                    BrokerCurrency = CurrencyType.EUR,
                                                                                    PriceVolumeUnitSpecified = true,
                                                                                    PriceVolumeUnit =
                                                                                        PriceVolumeUnitTypeElcertificate
                                                                                        .PerECS
                                                                                }


                                                                    };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, forward,
                                                                DealInsertTransactionDealType.ElcertificateForward);






        }

        public static DealInsert CreateElectricityGenConExcact(
            string porfolio,
            string couterpartyPorfolio,
            string priceBasis,
            DateTime fromDate,
            DateTime toDate,
            string financialPhysical,
            string currency,
            string currencySource,
            DateTime tradeDate,
            string externalId,
            string transactionName,
            DateTime[] fromDates,
            DateTime[] toDates,
            double[] volumes,
            double[] prices
            )
        {



          




            DealInsertGencon gencon = new DealInsertGencon
                                          {
                                              Portfolios = new DealInsertGenconPortfolios(){BuySellSpecified = false,Portfolio = porfolio,CounterpartyPortfolio = couterpartyPorfolio}

                                              ,Item = new DealInsertGenconElectricityGenconDetails()
                                                         {
                                                             ActivationDate = tradeDate,
                                                             PeriodFromDate = fromDate,
                                                             PeriodToDate = toDate,
                                                             PriceBasis = priceBasis,
                                                             TimezoneSpecified = false,
                                                             Status = GenconStatusType.Active,
                                                             ContractType = ElectricityGenConContractType.Retail,
                                                             Currency =DealInsertParser.GetCurrencyType(currency),
                                                             CurrencySource =DealInsertParser.GetCurrencySourceType(currencySource),
                                                             GenconName = transactionName,
                                                             MTMResolution = GenconMTMResolutionType.Hour,
                                                             DeliveryType =DealInsertParser.GenConDeliveryType(financialPhysical)
                                                         }
                                              ,ReferenceData = new DealInsertGenconReferenceData()
                                                                  {
                                                                      ExternalId = externalId,
                                                                      ExternalSource = "RegTest"
                                                                  }
                                              ,TimesSeries = new DealInsertGenconTimesSeries(){TimeSeriesDetail = DealInsertHelper.GenGenConTimeSeriesDetails(fromDates,toDates,volumes,prices)}


                                          };

            DealInsert transaction = new DealInsert()
                                                    {
                                                        Item = gencon

                                                        


                                                    };


            return transaction;

        }

        public static DealInsertTransaction CreateGasFloatingPriceIndexed(
           string porfolio,
           string couterpartyPorfolio,
           string buySell,
           string executionVenue,
           string priceBasis,
           DateTime fromDate,
           DateTime toDate,
           string financialPhysical,
           double price,
           double multiplicator,
           string indexFormulaName,
           string priceUnit,
           double quantity,
           string quanityUnit,
           string currency,
           string currencySource,
           DateTime tradeDate,
           string externalId,
            string balanceArea
           )
        {



            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             porfolio,
                                                                                                             couterpartyPorfolio);




            DealInsertTransactionGasFloatingPrice floatingPrice = new DealInsertTransactionGasFloatingPrice
            {
                InstrumentData =
                    new DealInsertTransactionGasFloatingPriceInstrumentData()
                    {
                        ExecutionVenue = ExecutionVenueType.None,
                            
                        FromDate = fromDate,
                        ToDate = toDate,
                    
                        PriceBasis = priceBasis,
                        TimezoneSpecified = false,
                        Item =
                            DealInsertParser.GetFinancialPhysical(
                                financialPhysical,balanceArea)
                        ,IndexFormulaName = indexFormulaName
                        ,PriceType = PriceTypeTypeGasFloatingPrice.Indexed
                        ,PriceTypeSpecified = true
                        

                    },
                SettlementData =
                    new DealInsertTransactionGasFloatingPriceSettlementData()
                    {
                       
                        Quantity = quantity,
                        Currency =
                            DealInsertParser.GetCurrencyType(currency),
                        CurrencySource =
                            DealInsertParser.GetCurrencySourceType(
                                currencySource),
                        Clearing = ClearingType.Electronic,
                        ClearingCurrency = CurrencyType.EUR,
                        BrokerCurrency = CurrencyType.EUR,
                        QuantityUnit =DealInsertParser.GetGasFloatingPriceQuantityUnit(quanityUnit),
                        PriceVolumeUnitSpecified = true,
                        PriceVolumeUnit =DealInsertParser.GetGasPriceUnit(priceUnit),
                        MarketPriceMultiplicator = multiplicator,
                        PriceMargin = price
                        

                    }


            };

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);


            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, floatingPrice,
                                                                DealInsertTransactionDealType.GasFloatingPrice);
        }

        public static DealInsertTransaction CreateElecticityReserveCapacity(string portfolio, string counterpartyPortfolio, string buySell, string priceBasis, DateTime fromDate, DateTime toDate, string deliveryType, double capacityBidPrice, double capacityBidQuantity, double price, double marketPriceMultiplicator, string priceVolumeUnit, double quantity, string quantityUnit, string currency, string currencySource, DateTime tradeDate, string externalId, string loadProfile, string transactionName, double capacityTradeQuantity, string resolution, DateTime[] fromDates, DateTime[] toDates, double[] volumes, string auctionType)
        {
            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell,
                                                                                                             portfolio,
                                                                                                             counterpartyPortfolio);



            DealInsertTransactionElectricityReserveCapacity reserveCapacity = new DealInsertTransactionElectricityReserveCapacity();

            DealInsertTransactionElectricityReserveCapacityInstrumentData instrumentData =
                new DealInsertTransactionElectricityReserveCapacityInstrumentData
                {
                    AuctionType = DealInsertParser.GetAuctionType(auctionType),
                    FromDate = fromDate,
                    ToDate = toDate,
                    LoadProfile = loadProfile,
                    Physical = new PhysicalTypeBalanceArea(),
                    PriceBasis = priceBasis,
                    TransactionName = transactionName
                };

            DealInsertTransactionElectricityReserveCapacitySettlementData settlementData = new DealInsertTransactionElectricityReserveCapacitySettlementData
            {
                CapacityPrice = capacityBidPrice,
                CapacityBidQuantity = capacityBidQuantity,
                CapacityTradeQuantity = capacityTradeQuantity,
                Currency = DealInsertParser.GetCurrencyType(currency),
                CurrencySource = DealInsertParser.GetCurrencySourceType(currencySource),
                Price = price,
                ReserveCapacityTimeSeries = DealInsertHelper.GetElectricityReserveCapacityTimeSeriesDetail(resolution, fromDates, toDates, volumes),
                QuantityUnit = QuantityUnitType.MW
            };


            reserveCapacity.InstrumentData = instrumentData;
            reserveCapacity.SettlementData = settlementData;

            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);
            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);

            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, reserveCapacity, DealInsertTransactionDealType.ElectricityReserveCapacity);
        }

        public static DealInsertTransaction CreateElectricitySpotSpotWithMargin(
            string portfolio,
            string counterpartyPortfolio, 
            string buySell, 
            string executionVenue,
            string priceBasis, 
            DateTime date, 
            string currency,
            string currencySource, 
            DateTime tradeDate, 
            string externalId, 
            string balanceArea,
            string resolution,
            string historicContractPrices,
            string historicMarketPrices,
            DateTime[] fromDates, 
            DateTime[] toDates, 
            double[] volumes, 
            double[] prices)
        {
            DealInsertTransactionElectricitySpot spot = new DealInsertTransactionElectricitySpot();

            PriceSeriesType contractPriceSeriesType = !string.IsNullOrEmpty(historicContractPrices) ? new PriceSeriesType {ExternalId = historicContractPrices } : null;
            PriceSeriesType marketPriceSeriesType = !string.IsNullOrEmpty(historicMarketPrices) ? new PriceSeriesType { ExternalId = historicMarketPrices} : null;
            DealInsertTransactionElectricitySpotInstrumentData instrumentData = new DealInsertTransactionElectricitySpotInstrumentData
            {
                ExecutionVenue = DealInsertParser.GetExecutionVenue(executionVenue),
                PriceBasis = priceBasis,
                TimezoneSpecified = false,
                Item = DealInsertParser.GetFinancialPhysical("Physical", balanceArea),
                Date = date,
                HistoricContractPrices = contractPriceSeriesType,
                HistoricMarketPrices = marketPriceSeriesType,
                PriceType = PriceTypeTypeElectricitySpot.Spot
                
            };
            DealInsertTransactionElectricitySpotSettlementData settlementData = new DealInsertTransactionElectricitySpotSettlementData
            {
                Currency = DealInsertParser.GetCurrencyType(currency),
                CurrencySource = DealInsertParser.GetCurrencySourceType(currencySource),
                Clearing = ClearingType.Electronic,
                ClearingCurrency = CurrencyType.EUR,
                BrokerCurrency = CurrencyType.EUR,
                Item = DealInsertHelper.GetElectricitySpotTimeSeriesDetail(resolution, fromDates, toDates, volumes, prices)
            };

            spot.InstrumentData = instrumentData;
            spot.SettlementData = settlementData;

            DealInsertTransactionPortfolios portfolios = DealInsertHelper.GetDealInsertTransactionPortfolios(buySell, portfolio, counterpartyPortfolio);
            DealInsertTransactionDealDetails details = DefaultTransactionDealDetails(tradeDate);
            DealInsertTransactionReferenceData referenceData = DefaultTransactionReferenceData(externalId);
            return DealInsertHelper.CreateDealInsertTransaction(portfolios, referenceData, details, spot, DealInsertTransactionDealType.ElectricitySpot);
        }

    }
}
