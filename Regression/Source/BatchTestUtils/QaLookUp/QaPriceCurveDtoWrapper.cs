using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ElvizTestUtils.CurveServiceReference;

namespace ElvizTestUtils.QaLookUp
{
   
    public class QaPriceCurveDtoWrapper
    {
        private readonly PriceCurveDto elvizDto;

        public QaPriceCurveDtoWrapper(PriceCurveDto elvizDto)
        {
            this.elvizDto = elvizDto;

        }

        public PriceCurveDto ElvizDto
        {
            get { return elvizDto; }
        }

        public string TimeZone { get { return this.elvizDto.TimeZone; } }

        public PriceCurveVolatilityDto PriceCurveVolatility
        {
            get { return this.elvizDto.PriceCurveVolatility; }
        }

        public string VolumeUnit { get { return this.elvizDto.VolumeUnit; } }


        public List<DateTimeValue> GetCurveValues()
        {
            List<DateTimeValue> dateTimevalues = this.elvizDto.ValueDates.Zip(this.elvizDto.Prices, (d, v) => new DateTimeValue(d, v)).ToList();
            return dateTimevalues;
        }

        public List<DateTimeValue> GetCurveValuesUTC()
        {
            List<DateTimeValue> dateTimevaluesUtc = this.elvizDto.ValueDatesUtc.Zip(this.elvizDto.Prices, (d, v) => new DateTimeValue(d, v)).ToList();
            return dateTimevaluesUtc;
        }

        public DataTable GetCurveValuesAsDataTable()
        {
            List<DateTimeValue> dateTimeValues = GetCurveValues();

            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));

            foreach (DateTimeValue dateTimeValue in dateTimeValues)
            {
                table.Rows.Add(new object[] {dateTimeValue.DateTime, dateTimeValue.Value});
            }

            return table;
        }

        public string CurveType
        {
            get
            {
                if (this.elvizDto.IsMarketPriceCurve && !this.elvizDto.IsIndexPriceCurve && !this.elvizDto.IsProxyPriceCurve)
                    return "MarketPriceCurve";

                if (!this.elvizDto.IsMarketPriceCurve && this.elvizDto.IsIndexPriceCurve && !this.elvizDto.IsProxyPriceCurve)
                    return "IndexPriceCurve";

                if (!this.elvizDto.IsMarketPriceCurve && !this.elvizDto.IsIndexPriceCurve && this.elvizDto.IsProxyPriceCurve)
                    return "ProxyPriceCurve";

                throw new ArgumentException("Curve DTO is neither MarketPrice, Index nor Proxy");
            }
        }
    }



    public class DateTimeValue
    {
        public DateTimeValue(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }

        public DateTime DateTime;
        public double Value;
    }

   
}
