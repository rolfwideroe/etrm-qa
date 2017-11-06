using System;

namespace DealInsertExtended
{
    public class DealInsertParser
    {
        public static PriceTypeTypeElectricityFloatingPrice GetElFloatingPriceType(string priceType)
        {
            PriceTypeTypeElectricityFloatingPrice myType;
            if (!Enum.TryParse(priceType, true, out myType))
                throw new ArgumentException(priceType + " : No such PriceType Exists");
            return myType;
        }

        public static PriceTypeType GetPriceType(string priceType)
        {
            PriceTypeType myType;
            if (!Enum.TryParse(priceType, true, out myType))
                throw new ArgumentException(priceType + " : No such PriceType Exists");
            return myType;
        }

        public static QuantityUnitTypeOil GetQuantityUnitTypeOil(string quantityType)
        {
            QuantityUnitTypeOil myType;
            if (!Enum.TryParse(quantityType, true, out myType))
                throw new ArgumentException(quantityType + " : No such quantity type Exists");
            return myType;
        }

        public static PriceVolumeUnitTypeOil GetPriceVolumeUnitTypeOil(string quantityType)
        {
            PriceVolumeUnitTypeOil myType;
            if (!Enum.TryParse(quantityType, true, out myType))
                throw new ArgumentException(quantityType + " : No such type Exists");
            return myType;
        }

        public static CurrencyType GetCurrencyType(string currencyType)
        {
            CurrencyType myType;
            if (!Enum.TryParse(currencyType, true, out myType))
                throw new ArgumentException(currencyType + " : No such Currency Exists");
            return myType;

        }

        public static BuySellType GetBuySellType(string buySellType)
        {
            BuySellType myType;
            if (!Enum.TryParse(buySellType, true, out myType))
                throw new ArgumentException(buySellType + " : No such BuySellType Exists");
            return myType;
        }


        public static ExecutionVenueType GetExecutionVenue(string executionVenue)
        {
            ExecutionVenueType myType;
            if (!Enum.TryParse(executionVenue, true, out myType))
                throw new ArgumentException(executionVenue + " : No such ExecutionVenue Exists");
            return myType;
        }

        public static CurrencySourceType GetCurrencySourceType(string currencySourceType)
        {
            CurrencySourceType myType;
            if (!Enum.TryParse(currencySourceType, true, out myType))
                throw new ArgumentException(currencySourceType + " : No such ExecutionVenue Exists");
            return myType;
        }

        public static PutCallType GetPutCallType(string putCallType)
        {
            PutCallType myType;
            if (!Enum.TryParse(putCallType, true, out myType))
                throw new ArgumentException(putCallType + " : No such PutCallType Exists");
            return myType;
        }

        public static SamplingPeriodType GetSamplingPeriodType(string samplingPeriodType)
        {
            SamplingPeriodType myType;
            if (!Enum.TryParse(samplingPeriodType, true, out myType))
                throw new ArgumentException(samplingPeriodType + " : No such samplingPeriodType Exists");
            return myType;
        }

        public static ElectricityStructuredDealResolutionType GetElectricityStructuredDealResolutionType(string type)
        {
            ElectricityStructuredDealResolutionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such ElectricityStructuredDealResolutionType Exists");
            return myType;
        }

        public static DealResolutionType GetDealResolutionType(string type)
        {
            DealResolutionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such DealResolutionType Exists");
            return myType;
        }

        public static BundleAsianDealResolutionType GetBundleAsianDealResolutionType(string type)
        {
            BundleAsianDealResolutionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such BundleAsianDealResolutionType Exists");
            return myType;
        }

        public static QuantityUnitTypeGas GetGasQuantityUnit(string type)
        {
            QuantityUnitTypeGas myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such GetGasQuantityUnit Exists");
            return myType;
        }

        public static QuantityUnitTypeGasFuture GetGasFutureQuantityUnit(string type)
        {
            QuantityUnitTypeGasFuture myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such GetGasQuantityUnit Exists");
            return myType;
        }
        
        public static PriceVolumeUnitTypeGasRestricted GetGasVolumeUnitRestricted(string type)
        {
            PriceVolumeUnitTypeGasRestricted myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such GetGasQuantityUnit Exists");
            return myType;
        }

        public static PriceVolumeUnitTypeGasExtended GetGasPriceUnit(string type)
        {
            PriceVolumeUnitTypeGasExtended myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such GetGasPriceUnit Exists");
            return myType;
        }

        public static PriceVolumeUnitTypeGas GetGasPriceVolumeUnit(string type)
        {
            PriceVolumeUnitTypeGas myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such GetGasPriceVolumeUnit Exists");
            return myType;
        }


        public static object GetFinancialPhysical(string financialPhysical,string balanceArea)
        {
            if (String.Equals("Financial", financialPhysical, StringComparison.CurrentCultureIgnoreCase))
                return new FinancialType();
            if (String.Equals("Physical", financialPhysical, StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(balanceArea)) return new PhysicalTypeBalanceArea();

                PhysicalTypeBalanceArea type = new PhysicalTypeBalanceArea();

                BalanceAreaType bal = new BalanceAreaType
                {
                    AreaName = balanceArea
                   // DeliveryArea = new BalanceAreaType() {AreaName = balanceArea}
                };

                type.DeliveryArea = bal;

                return  type;
            }
                

            throw new Exception(financialPhysical +" :No such type exists");
        }

        public static DeliveryType GenConDeliveryType(string financialPhysical)
        {
            if (String.Equals("Financial", financialPhysical, StringComparison.CurrentCultureIgnoreCase))
                return DeliveryType.Financial;
            if (String.Equals("Physical", financialPhysical, StringComparison.CurrentCultureIgnoreCase))
                return DeliveryType.Physical;

            throw new Exception(financialPhysical + " :No such type exists");
        }

        public static QuantityUnitTypeGasFloatingPrice GetGasFloatingPriceQuantityUnit(string quantityUnit)
        {
            QuantityUnitTypeGasFloatingPrice myType;
            if (!Enum.TryParse(quantityUnit, true, out myType))
                throw new ArgumentException(quantityUnit + " : No such QuantityUnitTypeGasFloatingPrice Exists");
            return myType;
        }

        public static TimeSeriesResolutionType GetTimeSeriesResolutionType(string type)
        {
            TimeSeriesResolutionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such TimeSeriesResolutionType Exists");
            return myType;
        }


        public static ElectricitySpotDealResolutionType GetSpotTimeSeriesResolutionType(string type)
        {
            ElectricitySpotDealResolutionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such ElectricitySpotDealResolutionType Exists");
            return myType;
        }

        public static ReserveCapacityAuctionType GetAuctionType(string type)
        {
            ReserveCapacityAuctionType myType;
            if (!Enum.TryParse(type, true, out myType))
                throw new ArgumentException(type + " : No such TimeSeriesResolutionType Exists");
            return myType;
        }
    }
}
