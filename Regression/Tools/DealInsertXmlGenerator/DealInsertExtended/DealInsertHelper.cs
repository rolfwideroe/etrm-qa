using System;
using System.Collections.Generic;
using System.Linq;

namespace DealInsertExtended
{
    public class DealInsertHelper
    {
        internal static DealInsertTransactionPortfolios GetDealInsertTransactionPortfolios(string buySell, string portfolio, string counterpartyPortfolio)
        {
            return new DealInsertTransactionPortfolios
            {
                Item = DealInsertParser.GetBuySellType(buySell),
                Portfolio = portfolio,
                CounterpartyPortfolio = counterpartyPortfolio

            };
        }

        public static DealInsert CreateDealInsert(DealInsertMetaData metaData, object transactionData)
        {
            return new DealInsert() { Item = transactionData, MetaData = metaData };

        }


        public static DealInsertTransaction CreateDealInsertTransaction(DealInsertTransactionPortfolios portfolios,
                                                                         DealInsertTransactionReferenceData referenceData,
                                                                         DealInsertTransactionDealDetails details,
                                                                         object transaction, DealInsertTransactionDealType transactionType)
        {
            return new DealInsertTransaction()
                       {
                           TransactionType = TransactionTypeType.Automatic,
                           DealDetails = details,
                           Item = transaction,
                           Portfolios = portfolios,
                           ReferenceData = referenceData,
                           DealType = transactionType
                       };
        }

        public static DealInsertTransactionElectricityStructuredDealSettlementDataStructuredDealInformation GetElectricityStructuredDealInformation(string loadProfile,string resolution,DateTime[] fromDateTimes,DateTime[] toDateTimes,double[] volumes,double[] prices,double[] volumesOutsideProfile,double[] pricesOutsideProfile)
        {
 

            TimeSeriesWithoutsideProfileDetailPeriodType[] timeSeries;

            if(loadProfile=="Base")
            {
                timeSeries = GetElectricityTimeSeriesWithoutProfile(fromDateTimes, toDateTimes, volumes, prices);
            }
            else
            {
                timeSeries = GetElectricityTimeSeries(fromDateTimes,toDateTimes,volumes,prices,volumesOutsideProfile,pricesOutsideProfile);
            }

            DealInsertTransactionElectricityStructuredDealSettlementDataStructuredDealInformation information =
                new DealInsertTransactionElectricityStructuredDealSettlementDataStructuredDealInformation()
                    {
                        Resolution = DealInsertParser.GetElectricityStructuredDealResolutionType(resolution),
                        TimeSeriesDetail = timeSeries
                    };

            return information;
        }

        public static FlexibleStructuredDealInformationType GetElectricityStructuredDealFlexInformation(string loadProfile, string resolution, DateTime[] fromDateTimes, DateTime[] toDateTimes, double[] forecastedVolumes, double[] forecastedPrices, double[] realizedVolumes, double[] realizedPrices)
        {
            List<TimeSeriesType> timeSeries = GetElectricityFlexibleTimeSeries(resolution, fromDateTimes, forecastedVolumes, forecastedPrices, realizedVolumes, realizedPrices);

            FlexibleStructuredDealInformationType information =
                new FlexibleStructuredDealInformationType()
                {
                    TimeSeries = timeSeries.ToArray()
                };

            return information;
        }

        private static List<TimeSeriesType> GetElectricityFlexibleTimeSeries(string resolution, DateTime[] fromDateTimes, double[] forecastedVolumes, double[] forecastedPrices, double[] realizedVolumes, double[] realizedPrices)
        {
            List<TimeSeriesType> timeSeries = new List<TimeSeriesType>();

            TimeSeriesType forecastedVolumeType = new TimeSeriesType
            {
                Resolution = DealInsertParser.GetTimeSeriesResolutionType(resolution),
                TimeSeriesType1 = "ForecastedVolume"
            };
            TimeSeriesType forecastedPriceType = new TimeSeriesType()
            {
                Resolution = DealInsertParser.GetTimeSeriesResolutionType(resolution),
                TimeSeriesType1 = "ForecastedPrice"
            };

            TimeSeriesType realizedVolumeType = new TimeSeriesType()
            {
                Resolution = DealInsertParser.GetTimeSeriesResolutionType(resolution),
                TimeSeriesType1 = "RealizedVolume"
            };

            TimeSeriesType realizedPriceType = new TimeSeriesType()
            {
                Resolution = DealInsertParser.GetTimeSeriesResolutionType(resolution),
                TimeSeriesType1 = "RealizedPrice"
            };


            List<TimeSeriesValueType> forecastedVolumesValueTypes = new List<TimeSeriesValueType>();
            List<TimeSeriesValueType> forecastedPricesValuesTypes = new List<TimeSeriesValueType>();
            List<TimeSeriesValueType> realizedVolumesValueTypes = new List<TimeSeriesValueType>();
            List<TimeSeriesValueType> realizedPricesValuesTypes = new List<TimeSeriesValueType>();
            for (int i = 0; i < fromDateTimes.Length; i++)
            {
                TimeSeriesValueType forecastedVolume = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTimeSpecified = false,
                    Value = forecastedVolumes[i],
                    ValueSpecified = true
                };

                TimeSeriesValueType forecastedPrice = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTimeSpecified = false,
                    Value = forecastedPrices[i],
                    ValueSpecified = true
                };

                TimeSeriesValueType realizedVolume = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTimeSpecified = false,
                    Value = realizedVolumes[i],
                    ValueSpecified = true
                };

                TimeSeriesValueType realizedPrice = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTimeSpecified = false,
                    Value = realizedPrices[i],
                    ValueSpecified = true
                };

                forecastedVolumesValueTypes.Add(forecastedVolume);
                forecastedPricesValuesTypes.Add(forecastedPrice);
                realizedVolumesValueTypes.Add(realizedVolume);
                realizedPricesValuesTypes.Add(realizedPrice);
            }
            forecastedVolumeType.Items = forecastedVolumesValueTypes.Select(x => x as object).ToArray();
            forecastedPriceType.Items = forecastedPricesValuesTypes.Select(x => x as object).ToArray();
            realizedVolumeType.Items = realizedVolumesValueTypes.Select(x => x as object).ToArray();
            realizedPriceType.Items = realizedPricesValuesTypes.Select(x => x as object).ToArray();

            timeSeries.Add(forecastedVolumeType);
            timeSeries.Add(forecastedPriceType);
            timeSeries.Add(realizedVolumeType);
            timeSeries.Add(realizedPriceType);

            return timeSeries;
        }


        public static DealInsertTransactionElectricityEuropeanSettlementDataQuantitySeries GetElectricityEuropeanTimeSeriesDetail(string resolution, DateTime[] fromDateTimes, DateTime[] toDateTimes, double[] volumes)
        {
            IList<TimeSeriesValueType> timeSeries = new List<TimeSeriesValueType>();

            for (int i = 0; i < fromDateTimes.Count(); i++)
            {
                TimeSeriesValueType timeSeriesValueType = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTime = toDateTimes[i],
                    ToDateTimeSpecified = true,
                    Value = volumes[i]
                };

                timeSeries.Add(timeSeriesValueType);
            }

            DealInsertTransactionElectricityEuropeanSettlementDataQuantitySeries type = new DealInsertTransactionElectricityEuropeanSettlementDataQuantitySeries
            {
                Resolution = DealInsertParser.GetDealResolutionType(resolution),
                TimeSeriesDetail = timeSeries.ToArray()
            };

            return type;
        }

        public static DealInsertTransactionElectricityAsianSettlementDataQuantitySeries GetElectricityAsianTimeSeriesDetail(string resolution, DateTime[] fromDateTimes, DateTime[] toDateTimes, double[] volumes)
        {
            IList<TimeSeriesValueType> timeSeries = new List<TimeSeriesValueType>();

            for (int i = 0; i < fromDateTimes.Count(); i++)
            {
                TimeSeriesValueType timeSeriesValueType = new TimeSeriesValueType
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTime = toDateTimes[i],
                    Value = volumes[i],
                    ToDateTimeSpecified = true,
                    ValueSpecified = true
                    
                };

                timeSeries.Add(timeSeriesValueType);
            }

            DealInsertTransactionElectricityAsianSettlementDataQuantitySeries type = new DealInsertTransactionElectricityAsianSettlementDataQuantitySeries
            {
                Resolution = DealInsertParser.GetBundleAsianDealResolutionType(resolution),
                TimeSeriesDetail = timeSeries.ToArray()
            };

            return type;
        }


        public static TimeSeriesWithoutsideProfileDetailPeriodType[] GetElectricityTimeSeriesWithoutProfile(DateTime[] fromDateTimes,DateTime[] toDateTimes,double[] volumes,double[] prices)
        {
            if(new[]{fromDateTimes.Count(),toDateTimes.Count(),volumes.Count(),prices.Count()}.All(x=> x ==fromDateTimes.Count()))
            {
                int length = fromDateTimes.Count();
                TimeSeriesWithoutsideProfileDetailPeriodType[] timeSeries =
                    new TimeSeriesWithoutsideProfileDetailPeriodType[length];

                for (int i = 0; i < length; i++)
                {
                    timeSeries[i]=GetTimeSeriesWithoutsideProfileDetailPeriodType(fromDateTimes[i],toDateTimes[i],prices[i],volumes[i]);
                }

                return timeSeries;
            }

            throw new Exception("no good");
        }

        public static DealInsertGenconTimesSeriesTimeSeriesDetail[] GenGenConTimeSeriesDetails(DateTime[] fromDateTimes, DateTime[] toDateTimes, double[] volumes, double[] prices)
        {
            if (new[] { fromDateTimes.Count(), toDateTimes.Count(), volumes.Count(), prices.Count() }.All(x => x == fromDateTimes.Count()))
            {
                int length = fromDateTimes.Count();
                DealInsertGenconTimesSeriesTimeSeriesDetail[] timeSeries =
                    new DealInsertGenconTimesSeriesTimeSeriesDetail[length*2];

                bool isForeCast = false;
                int index = 0;

                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        timeSeries[index] = new DealInsertGenconTimesSeriesTimeSeriesDetail() { FromDate = fromDateTimes[i], ToDate = toDateTimes[i].AddSeconds(-1), Forecast = isForeCast, Volume = volumes[i], Price = prices[i] };
                        index++;
                    }
                    isForeCast = true;
                }
                return timeSeries;
            }

            throw new Exception("no good");
        }       
        
        public static TimeSeriesWithoutsideProfileDetailPeriodType[] GetElectricityTimeSeries(DateTime[] fromDateTimes,DateTime[] toDateTimes,double[] volumes,double[] prices,double[] volumesOutsideProfile,double[] pricesOutsideProfile)
        {
            if(new[]{fromDateTimes.Count(),toDateTimes.Count(),volumes.Count(),prices.Count()}.All(x=> x ==fromDateTimes.Count()))
            {
                int length = fromDateTimes.Count();
                TimeSeriesWithoutsideProfileDetailPeriodType[] timeSeries =
                    new TimeSeriesWithoutsideProfileDetailPeriodType[length];

                for (int i = 0; i < length; i++)
                {
                    timeSeries[i]=GetElectricityTimeSeriesDetail(fromDateTimes[i],toDateTimes[i],prices[i],volumes[i],volumesOutsideProfile[i],pricesOutsideProfile[i]);
                }

                return timeSeries;
            }

            throw new Exception("no good");
        }

        public static TimeSeriesWithoutsideProfileDetailPeriodType GetElectricityTimeSeriesDetail(DateTime fromDateTime, DateTime toDateTime, double volume, double price, double volumeOutsideProfile, double priceOutsideProfile)
    { 
        TimeSeriesWithoutsideProfileDetailPeriodType type = new TimeSeriesWithoutsideProfileDetailPeriodType
                                                                {
                                                                    FromDateTime = fromDateTime,
                                                                    ToDateTime = toDateTime,
                                                                    Price = price,
                                                                    Volume = volume,
                                                                    PriceOutsideProfile = priceOutsideProfile,
                                                                    PriceOutsideProfileSpecified = true,
                                                                    VolumeOutsideProfile = volumeOutsideProfile,
                                                                    VolumeOutsideProfileSpecified = true
                                                                };


        return type;
    }
        
    public static TimeSeriesWithoutsideProfileDetailPeriodType GetTimeSeriesWithoutsideProfileDetailPeriodType(DateTime fromDateTime, DateTime toDateTime, double price, double volume)
    {
        TimeSeriesWithoutsideProfileDetailPeriodType type = new TimeSeriesWithoutsideProfileDetailPeriodType
                                                                {
                                                                    FromDateTime = fromDateTime,
                                                                    ToDateTime = toDateTime,
                                                                    Price = price,
                                                                    Volume = volume,
                                                                    PriceOutsideProfileSpecified = false,
                                                                    VolumeOutsideProfileSpecified = false
                                                                };


        return type;
    }

    public static DealInsertTransactionGasSwapSettlementDataPriceVolumeTimeSeries GetGasSwapTimeSeries(DateTime[] fromDateTimes,DateTime[] toDateTimes,double[] volumes,double[] prices)
    {
        DealInsertTransactionGasSwapSettlementDataPriceVolumeTimeSeries series=new DealInsertTransactionGasSwapSettlementDataPriceVolumeTimeSeries();
        if(new[]{fromDateTimes.Count(),toDateTimes.Count(),volumes.Count(),prices.Count()}.All(x=> x ==fromDateTimes.Count()))
            {
                int length = fromDateTimes.Count();
                TimeSeriesDetailPeriodType[] timeSeries =
                    new TimeSeriesDetailPeriodType[length];

                for (int i = 0; i < length; i++)
                {
                    timeSeries[i] = new TimeSeriesDetailPeriodType()
                                        {
                                            FromDateTime = fromDateTimes[i],
                                            ToDateTime = toDateTimes[i],
                                            Volume = volumes[i],
                                            Price = prices[i]
                                        };
                }

                series.TimeSeriesDetail = timeSeries;
                return series;
            }

        
            throw new Exception("no good");


        
    }

    public static DealInsertTransactionOilSwapSettlementDataPriceVolumeTimeSeries GetOilSwapTimeSeries(DateTime[] fromDateTimes, DateTime[] toDateTimes, double[] volumes, double[] prices) 
    {
        DealInsertTransactionOilSwapSettlementDataPriceVolumeTimeSeries series = new DealInsertTransactionOilSwapSettlementDataPriceVolumeTimeSeries();
        if (new[] { fromDateTimes.Count(), toDateTimes.Count(), volumes.Count(), prices.Count() }.All(x => x == fromDateTimes.Count()))
        {
            int length = fromDateTimes.Count();
            TimeSeriesDetailPeriodType[] timeSeries =
                new TimeSeriesDetailPeriodType[length];

            for (int i = 0; i < length; i++)
            {
                timeSeries[i] = new TimeSeriesDetailPeriodType()
                {
                    FromDateTime = fromDateTimes[i],
                    ToDateTime = toDateTimes[i],
                    Volume = volumes[i],
                    Price = prices[i]
                };
            }

            series.TimeSeriesDetail = timeSeries;
            return series;
        }


        throw new Exception("no good");



    }


        public static FlexibleStructuredDealInformationType GetElectricityReserveCapacityTimeSeriesDetail(string resolution, DateTime[] fromDates, DateTime[] toDates,
            double[] volumes)
        {
            FlexibleStructuredDealInformationType flexibleStructuredDealInformationType = new FlexibleStructuredDealInformationType();

            TimeSeriesType[] timeSeriesTypes = new TimeSeriesType[1];
            TimeSeriesType timeSeriesType = new TimeSeriesType
            {
                Resolution = DealInsertParser.GetTimeSeriesResolutionType(resolution),
                TimeSeriesType1 = "RealizedVolume"
            };

            List<TimeSeriesValueType> timeSeriesValueTypes = new List<TimeSeriesValueType>();

            int numberOfValues = volumes.Length;
            for (int i = 0; i < numberOfValues; i++)
            {
                TimeSeriesValueType timeSeriesValueType = new TimeSeriesValueType
                {
                    FromDateTime = fromDates[i],
                    ToDateTime = toDates[i],
                    ToDateTimeSpecified = true,
                    Value = volumes[i],
                    ValueSpecified = true
                };
                timeSeriesValueTypes.Add(timeSeriesValueType);
            }

            IEnumerable<object> items = timeSeriesValueTypes;

            timeSeriesType.Items = items.ToArray();
            timeSeriesTypes[0] = timeSeriesType;

            flexibleStructuredDealInformationType.TimeSeries = timeSeriesTypes;

            return flexibleStructuredDealInformationType;
        }

        public static object GetElectricitySpotTimeSeriesDetail(string resolution, DateTime[] fromDates, DateTime[] toDates, double[] volumes, double[] prices)
        {
            List<TimeSeriesDetailType> detailTypes = new List<TimeSeriesDetailType>();

            for (int i = 0; i < volumes.Length; i++)
            {
                TimeSeriesDetailType detailType = new TimeSeriesDetailType
                {
                    DateTime = fromDates[i],
                    Price = prices[i],
                    Volume = volumes[i]
                };
                detailTypes.Add(detailType);
            }

            DealInsertTransactionElectricitySpotSettlementDataTimeSeriesInformation info = new DealInsertTransactionElectricitySpotSettlementDataTimeSeriesInformation
            {
                Resolution = DealInsertParser.GetSpotTimeSeriesResolutionType(resolution),
                TimeSeriesDetail = detailTypes.ToArray()
            };

            return info;
        }
    }
}
