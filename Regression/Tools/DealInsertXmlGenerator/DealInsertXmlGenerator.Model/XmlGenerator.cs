using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DealInsertExtended;

namespace DealInsertXmlGenerator.Model
{
    public class XmlGenerator
    {
        private readonly IList<DealInsert> inserts;
        private readonly TransactionWrapperType type;
        private readonly DealInsertMetaData metaData;

        public XmlGenerator(TransactionWrapperType type)
        {
            this.type = type;
            inserts = new List<DealInsert>();
            metaData = DealInsertFactory.CreateDefaultDealInsertMetaData();
        }



        public void AddWrapper(TransactionWrapper wrapper)
        {
            DealInsertTransaction transaction=new DealInsertTransaction();

            if (this.type == TransactionWrapperType.ElectricityForward)
            {
                transaction = DealInsertFactory.CreateElectricityForward(wrapper.Portfolio,
                                                                              wrapper.CounterpartyPortfolio,
                                                                              wrapper.BuySell,
                                                                              wrapper.ExecutionVenue,
                                                                              wrapper.PriceBasis,
                                                                              wrapper.LoadProfile,
                                                                              wrapper.FromDate,
                                                                              wrapper.ToDate,
                                                                              wrapper.DeliveryType,
                                                                              wrapper.Price,
                                                                              wrapper.Quantity,
                                                                              wrapper.Currency,
                                                                              wrapper.CurrencySource,
                                                                              wrapper.TradeDate,
                                                                              wrapper.ExternalId,
                                                                     wrapper.BalanceArea);

            }

            if (this.type == TransactionWrapperType.ElectricityFuture)
            {
                transaction = DealInsertFactory.CreateElectricityFuture(wrapper.Portfolio,
                                                              wrapper.CounterpartyPortfolio,
                                                              wrapper.BuySell,
                                                              wrapper.ExecutionVenue,
                                                              wrapper.PriceBasis,
                                                              wrapper.LoadProfile,
                                                              wrapper.FromDate,
                                                              wrapper.ToDate,
                                                              wrapper.DeliveryType,
                                                              wrapper.Price,
                                                              wrapper.Quantity,
                                                              wrapper.Currency,
                                                              wrapper.CurrencySource,
                                                              wrapper.TradeDate,
                                                              wrapper.ExternalId,
                                                                     wrapper.BalanceArea);


            }

            if(this.type==TransactionWrapperType.CurrencyForward)
            {
                transaction = DealInsertFactory.CreateCurrencyForward(wrapper.Portfolio,
                                                                           wrapper.CounterpartyPortfolio,
                                                                           wrapper.BuySell,
                                                                           wrapper.BaseCurrency,
                                                                           wrapper.CrossCurrency,
                                                                           wrapper.DeliveryDate,
                                                                           wrapper.Price,
                                                                           wrapper.Quantity,
                                                                           wrapper.CurrencySource,
                                                                           wrapper.TradeDate,
                                                                           wrapper.ExternalId);
            }

            if(this.type==TransactionWrapperType.ElectricityFloating)
            {
                transaction = DealInsertFactory.CreateElectricityFloatingPrice(wrapper.Portfolio,
                                                                               wrapper.CounterpartyPortfolio,
                                                                               wrapper.BuySell,
                                                                               wrapper.ExecutionVenue,
                                                                               wrapper.PriceBasis,
                                                                               wrapper.LoadProfile,
                                                                               wrapper.FromDate,
                                                                               wrapper.ToDate,
                                                                               wrapper.DeliveryType,
                                                                               wrapper.PriceType,
                                                                               wrapper.IndexFormulaName,
                                                                               wrapper.MarketPriceMultiplicator,
                                                                               wrapper.Price, 
                                                                               wrapper.Quantity,
                                                                               wrapper.Currency,
                                                                               wrapper.CurrencySource,
                                                                               wrapper.TradeDate,
                                                                               wrapper.ExternalId,
                                                                     wrapper.BalanceArea);
            }

            if(this.type==TransactionWrapperType.OilFuture)
            {
                transaction = DealInsertFactory.CreateOilFuture(wrapper.Portfolio, 
                                                                wrapper.CounterpartyPortfolio,
                                                                wrapper.BuySell, 
                                                                wrapper.ExecutionVenue,
                                                                wrapper.PriceBasis, 
                                                                wrapper.FromDate, 
                                                                wrapper.ToDate,
                                                                wrapper.DeliveryType, 
                                                                wrapper.PriceBasis, 
                                                                wrapper.Price,
                                                                wrapper.Quantity, 
                                                                wrapper.QuantityUnit,
                                                                wrapper.Currency,
                                                                wrapper.CurrencySource,
                                                                wrapper.TradeDate,
                                                                wrapper.ExternalId);
            }

            if(this.type==TransactionWrapperType.ElectricityAsian)
            {
                if (wrapper.TimeSeriesDetailWrappers == null)
                transaction = DealInsertFactory.CreateElectricityAsian(wrapper.Portfolio, 
                                                                       wrapper.CounterpartyPortfolio,
                                                                       wrapper.BuySell,
                                                                       wrapper.ExecutionVenue,
                                                                       wrapper.PriceBasis,
                                                                       wrapper.LoadProfile,
                                                                       wrapper.FromDate,
                                                                       wrapper.ToDate,
                                                                       wrapper.DeliveryType,
                                                                       wrapper.PutCall,
                                                                       wrapper.Strike,
                                                                       wrapper.SamplingPeriod,
                                                                       wrapper.Price, 
                                                                       wrapper.Quantity, 
                                                                       wrapper.Currency,
                                                                       wrapper.CurrencySource, 
                                                                       wrapper.TradeDate,
                                                                       wrapper.ExternalId,
                                                                     wrapper.BalanceArea,
                                                                          null, null, null, string.Empty); //No structured asian 
                else
                    transaction = DealInsertFactory.CreateElectricityAsian(wrapper.Portfolio,
                                                                           wrapper.CounterpartyPortfolio,
                                                                           wrapper.BuySell,
                                                                           wrapper.ExecutionVenue,
                                                                           wrapper.PriceBasis,
                                                                           wrapper.LoadProfile,
                                                                           wrapper.FromDate,
                                                                           wrapper.ToDate,
                                                                           wrapper.DeliveryType,
                                                                           wrapper.PutCall,
                                                                           wrapper.Strike,
                                                                           wrapper.SamplingPeriod,
                                                                           wrapper.Price,
                                                                           wrapper.Quantity,
                                                                           wrapper.Currency,
                                                                           wrapper.CurrencySource,
                                                                           wrapper.TradeDate,
                                                                           wrapper.ExternalId,
                                                                         wrapper.BalanceArea, 
                                                                         wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                                                                          wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                                                                          wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                                                                          wrapper.Resolution);

            }

            if (this.type == TransactionWrapperType.ElectricityStructured)
            {
                if (wrapper.ModelType == "Flexible")
                {
                    transaction = DealInsertFactory.CreateElectricityStructuredDealFlex(wrapper.Portfolio,
                        wrapper.CounterpartyPortfolio,
                        wrapper.BuySell,
                        wrapper.ExecutionVenue,
                        wrapper.PriceBasis,
                        wrapper.LoadProfile,
                        wrapper.FromDate,
                        wrapper.ToDate,
                        wrapper.DeliveryType,
                        wrapper.Currency,
                        wrapper.CurrencySource,
                        wrapper.TradeDate,
                        wrapper.ExternalId,
                        wrapper.TransactionName,
                        wrapper.Resolution,
                        wrapper.PriceType,
                        wrapper.HistoricContractPrices,
                        wrapper.HistoricMarketPrices,
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.RealizedVolume).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.RealizedPrice).ToArray(),
                        wrapper.BalanceArea);
                }
                else
                {

                    transaction = DealInsertFactory.CreateElectricityStructuredDealFixed(wrapper.Portfolio,
                        wrapper.CounterpartyPortfolio,
                        wrapper.BuySell,
                        wrapper.ExecutionVenue,
                        wrapper.PriceBasis,
                        wrapper.LoadProfile,
                        wrapper.FromDate,
                        wrapper.ToDate,
                        wrapper.DeliveryType,
                        wrapper.Currency,
                        wrapper.CurrencySource,
                        wrapper.TradeDate,
                        wrapper.ExternalId,
                        wrapper.TransactionName,
                        wrapper.Resolution,
                        wrapper.PriceType,
                        wrapper.HistoricContractPrices,
                        wrapper.HistoricMarketPrices,
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.VolumeOutsideProfile).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.VolumeOutsideProfile).ToArray(),
                        wrapper.BalanceArea);
                }
            }

            if (this.type==TransactionWrapperType.EmissionFuture)
            {
                transaction = DealInsertFactory.CreateEmissionFuture(wrapper.Portfolio,
                                                                     wrapper.CounterpartyPortfolio,
                                                                     wrapper.BuySell,
                                                                     wrapper.ExecutionVenue,
                                                                     wrapper.PriceBasis,
                                                                     wrapper.DeliveryDate,
                                                                     wrapper.Price,
                                                                     wrapper.Quantity,
                                                                     wrapper.Currency,
                                                                     wrapper.CurrencySource,
                                                                     wrapper.TradeDate,
                                                                     wrapper.ExternalId
                                                                     );
            }

            if(this.type==TransactionWrapperType.GasForward)
            {
                transaction = DealInsertFactory.CreateGasForward(wrapper.Portfolio,
                                                                     wrapper.CounterpartyPortfolio,
                                                                     wrapper.BuySell,
                                                                     wrapper.ExecutionVenue,
                                                                     wrapper.PriceBasis,
                                                                     wrapper.FromDate,
                                                                     wrapper.ToDate,
                                                                     wrapper.DeliveryType,
                                                                     wrapper.Price,
                                                                     wrapper.PriceVolumeUnit,
                                                                     wrapper.Quantity,
                                                                     wrapper.QuantityUnit,
                                                                     wrapper.Currency,
                                                                     wrapper.CurrencySource,
                                                                     wrapper.TradeDate,
                                                                     wrapper.ExternalId,
                                                                     wrapper.BalanceArea
                                                                     );
            }

            if (this.type == TransactionWrapperType.GasFuture)
            {
                transaction = DealInsertFactory.CreateGasFuture(wrapper.Portfolio,
                                                                     wrapper.CounterpartyPortfolio,
                                                                     wrapper.BuySell,
                                                                     wrapper.ExecutionVenue,
                                                                     wrapper.PriceBasis,
                                                                     wrapper.FromDate,
                                                                     wrapper.ToDate,
                                                                     wrapper.DeliveryType,
                                                                     wrapper.Price,
                                                                     wrapper.PriceVolumeUnit,
                                                                     wrapper.Quantity,
                                                                     wrapper.QuantityUnit,
                                                                     wrapper.Currency,
                                                                     wrapper.CurrencySource,
                                                                     wrapper.TradeDate,
                                                                     wrapper.ExternalId,
                                                                     wrapper.BalanceArea
                                                                     );
            }

            if(this.type==TransactionWrapperType.GasSwap)
            {
                transaction = DealInsertFactory.CreateGasSwap(wrapper.Portfolio, 
                                                              wrapper.CounterpartyPortfolio,
                                                              wrapper.BuySell,
                                                              wrapper.ExecutionVenue,
                                                              wrapper.IndexFormulaName, 
                                                              wrapper.FromDate,
                                                              wrapper.ToDate,
                                                              wrapper.PriceVolumeUnit, 
                                                              wrapper.QuantityUnit,
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray(),
                                                              wrapper.Currency,
                                                              wrapper.CurrencySource,
                                                              wrapper.TradeDate, 
                                                              wrapper.ExternalId);
            }

            if (this.type == TransactionWrapperType.OilSwap)
            {
                transaction = DealInsertFactory.CreateOilSwap(wrapper.Portfolio,
                                                              wrapper.CounterpartyPortfolio,
                                                              wrapper.BuySell,
                                                              wrapper.ExecutionVenue,
                                                              wrapper.IndexFormulaName,
                                                              wrapper.FromDate,
                                                              wrapper.ToDate,
                                                              wrapper.PriceVolumeUnit,
                                                              wrapper.QuantityUnit,
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray(),
                                                              wrapper.Currency,
                                                              wrapper.CurrencySource,
                                                              wrapper.TradeDate,
                                                              wrapper.ExternalId);
            }


            if(this.type==TransactionWrapperType.ElectricityEuropean)
            {
                if (wrapper.TimeSeriesDetailWrappers == null)
                    transaction = DealInsertFactory.CreateElectricityEuropean(wrapper.Portfolio,
                                                                          wrapper.CounterpartyPortfolio, 
                                                                          wrapper.BuySell,
                                                                          wrapper.ExecutionVenue, 
                                                                          wrapper.PriceBasis,
                                                                          wrapper.LoadProfile, 
                                                                          wrapper.FromDate,
                                                                          wrapper.ToDate, 
                                                                          wrapper.ExpiryDate,
                                                                          wrapper.DeliveryType, 
                                                                          wrapper.PutCall,
                                                                          wrapper.Price, 
                                                                          wrapper.Strike,
                                                                          wrapper.Quantity, 
                                                                          wrapper.Currency,
                                                                          wrapper.CurrencySource, 
                                                                          wrapper.TradeDate,
                                                                          wrapper.ExternalId,
                                                                          wrapper.BalanceArea,
                                                                          wrapper.UnderlyingInstrumentType,
                                                                          null, null, null, string.Empty); //No underlying structured deal 
                else
                    transaction = DealInsertFactory.CreateElectricityEuropean(wrapper.Portfolio,
                                                                          wrapper.CounterpartyPortfolio, 
                                                                          wrapper.BuySell,
                                                                          wrapper.ExecutionVenue, 
                                                                          wrapper.PriceBasis,
                                                                          wrapper.LoadProfile, 
                                                                          wrapper.FromDate,
                                                                          wrapper.ToDate, 
                                                                          wrapper.ExpiryDate,
                                                                          wrapper.DeliveryType, 
                                                                          wrapper.PutCall,
                                                                          wrapper.Price, 
                                                                          wrapper.Strike,
                                                                          wrapper.Quantity, 
                                                                          wrapper.Currency,
                                                                          wrapper.CurrencySource, 
                                                                          wrapper.TradeDate,
                                                                          wrapper.ExternalId,
                                                                          wrapper.BalanceArea,
                                                                          wrapper.UnderlyingInstrumentType,
                                                                          wrapper.TimeSeriesDetailWrappers.Select(x=>x.FromDate).ToArray(), 
                                                                          wrapper.TimeSeriesDetailWrappers.Select(x=>x.ToDate).ToArray(), 
                                                                          wrapper.TimeSeriesDetailWrappers.Select(x=>x.Volume).ToArray(),
                                                                          wrapper.Resolution);
            }

            if (this.type == TransactionWrapperType.ElCertForward)
            {
                transaction = DealInsertFactory.CreateElCertForward(wrapper.Portfolio,
                                                                          wrapper.CounterpartyPortfolio,
                                                                          wrapper.BuySell,
                                                                          wrapper.ExecutionVenue,
                                                                          wrapper.PriceBasis,
                                                                          wrapper.DeliveryDate,
                                                                          wrapper.Price,
                                                                          wrapper.Quantity,
                                                                          wrapper.Currency,
                                                                          wrapper.CurrencySource,
                                                                          wrapper.TradeDate,
                                                                          wrapper.ExternalId);
            }


            if(this.type==TransactionWrapperType.GasFloatingPrice)
            {
                transaction = DealInsertFactory.CreateGasFloatingPriceIndexed(wrapper.Portfolio,
                                                                              wrapper.CounterpartyPortfolio,
                                                                              wrapper.BuySell, wrapper.ExecutionVenue,
                                                                              wrapper.PriceBasis, wrapper.FromDate,
                                                                              wrapper.ToDate, wrapper.DeliveryType,
                                                                              wrapper.Price,
                                                                              wrapper.MarketPriceMultiplicator,
                                                                              wrapper.IndexFormulaName,
                                                                              wrapper.PriceVolumeUnit, wrapper.Quantity,
                                                                              wrapper.QuantityUnit, wrapper.Currency,
                                                                              wrapper.CurrencySource, wrapper.TradeDate,
                                                                              wrapper.ExternalId,
                                                                     wrapper.BalanceArea);
            }

            if (this.type == TransactionWrapperType.GenCon)
            {
                DealInsert gencon = DealInsertFactory.CreateElectricityGenConExcact(wrapper.Portfolio,
                                                                              wrapper.CounterpartyPortfolio,
                                                                              wrapper.PriceBasis, wrapper.FromDate,
                                                                              wrapper.ToDate, wrapper.DeliveryType,
                                                                              wrapper.Currency, wrapper.CurrencySource,
                                                                              wrapper.TradeDate, wrapper.ExternalId,
                                                                              wrapper.TransactionName,
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray());

                gencon.MetaData = this.metaData;
                inserts.Add(gencon);
                return;

            }

            if (this.type == TransactionWrapperType.ElectricityReserveCapacity)
            {
                if (wrapper.TimeSeriesDetailWrappers != null)
                    transaction = DealInsertFactory.CreateElecticityReserveCapacity(wrapper.Portfolio,
                        wrapper.CounterpartyPortfolio,
                        wrapper.BuySell, 
                        wrapper.PriceBasis, 
                        wrapper.FromDate,
                        wrapper.ToDate, 
                        wrapper.DeliveryType,
                        wrapper.CapacityBidPrice,
                        wrapper.CapacityBidVolume,
                        wrapper.Price,
                        wrapper.MarketPriceMultiplicator,
                        wrapper.PriceVolumeUnit, 
                        wrapper.Quantity,
                        wrapper.QuantityUnit, 
                        wrapper.Currency,
                        wrapper.CurrencySource, 
                        wrapper.TradeDate,
                        wrapper.ExternalId,
                        wrapper.LoadProfile,
                        wrapper.TransactionName, 
                        wrapper.CapacityTradeVolume, 
                        wrapper.Resolution,
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                        wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(), 
                        wrapper.AuctionType);
            }

            if (this.type == TransactionWrapperType.ElectricitySpot)
            {
                transaction = DealInsertFactory.CreateElectricitySpotSpotWithMargin(wrapper.Portfolio,
                                                                              wrapper.CounterpartyPortfolio,
                                                                              wrapper.BuySell,
                                                                              wrapper.ExecutionVenue,
                                                                              wrapper.PriceBasis,
                                                                              wrapper.FromDate,
                                                                              wrapper.Currency,
                                                                              wrapper.CurrencySource,
                                                                              wrapper.TradeDate,
                                                                              wrapper.ExternalId,
                                                                              wrapper.BalanceArea,
                                                                              wrapper.Resolution,
                                                                              wrapper.HistoricContractPrices,
                                                                              wrapper.HistoricMarketPrices,
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.FromDate).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.ToDate).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Volume).ToArray(),
                                                                              wrapper.TimeSeriesDetailWrappers.Select(x => x.Price).ToArray());

            }



            DealInsert insert = DealInsertHelper.CreateDealInsert(this.metaData, transaction);
            inserts.Add(insert);
        }






        public void GenerateBulkInsertXmlFile(string filepath)
        {
            if (inserts.Count == 0)
                throw new Exception("No transactions");

            if(File.Exists(filepath))
                File.Delete(filepath);

            BulkDealInsert bulk = new BulkDealInsert() {DealInsert = this.inserts.ToArray()};

            XmlSerializer xmlSerializer = new XmlSerializer(typeof (BulkDealInsert));
            FileStream fileStream = File.Open(
                filepath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite);

            xmlSerializer.Serialize(fileStream, bulk);

            fileStream.Close();
            fileStream.Dispose();

        }

    }
}
    