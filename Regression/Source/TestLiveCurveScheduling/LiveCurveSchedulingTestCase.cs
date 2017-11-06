using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestLiveCurveScheduling
{

    public class LiveCurveSchedulingTestCase
    {
        public string Venue { get; set; }
        public string Couterparty { get; set; }
        public string Commodity { get; set; }
        public string RefArea { get; set; }
        public string TimeZone { get; set; }
        public string Feed { get; set; }
        public bool IsOption { get; set; }
        public bool IsSwap { get; set; }
        public string Currency { get; set; }

        public LivePrice[] LivePrices { get; set; }
    }

    

    public class LivePrice
    {
        public string LoadType { get; set; }

        public string TickerRule { get; set; }

        public int ReportDateOffSet { get; set; }

        public DateTime? CustomFromToDate { get; set; }

        public string PeriodType { get; set; }

        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Last { get; set; }
        public double Close { get; set; }

        [XmlIgnore]
        public string Ticker {
            get
            {

                if (PeriodType.ToUpper() == "CUSTOMPERIOD")
                {
                    if (CustomFromToDate == null) throw new ArgumentException("Customperiod requres CustomFrom/ToDate");
                    if (CustomFromToDate == null) throw new ArgumentException("Customperiod requres CustomFrom/ToDate");
                    return TickerParser.ParseCustomPeriod(TickerRule, CustomFromToDate.Value);

                }
                return TickerParser.Parse(TickerRule, PeriodType, ReportDateOffSet);
;
            }  }
        [XmlIgnore]
        public DateTime FromDateTime {
            get
            {
                if (PeriodType.ToUpper() == "CUSTOMPERIOD")
                {
                    if (CustomFromToDate == null) throw new ArgumentException("Customperiod requres CustomFrom/ToDate");
                    return CustomFromToDate.Value;
   

                }
                return TickerParser.GetStartDate(DateTime.Today, PeriodType, ReportDateOffSet);
            }  }

        [XmlIgnore]
        public DateTime ToDateTime {
            get
            {
                if (PeriodType.ToUpper() == "CUSTOMPERIOD")
                {
                    if (CustomFromToDate == null) throw new ArgumentException("Customperiod requres CustomFrom/ToDate");
                    if (CustomFromToDate == null) throw new ArgumentException("Customperiod requres CustomFrom/ToDate");
                    return CustomFromToDate.Value;


                }
                return TickerParser.GetToDate(DateTime.Today, PeriodType, ReportDateOffSet);
            }  }
    }
}
