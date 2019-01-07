using System;
using ElvizTestUtils.CurveServiceReference;

namespace ElvizTestUtils.CurveTests
{
    public class CurveInputData
    {
        public ElvizConfiguration[] ElvizConfigurations { get; set; }
        public string CurrencyQuote { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public RelativePeriod RelativePeriod { get; set; }
        public string LoadType { get; set; }
        public string PriceBookAppendix { get; set; }
        public string ReferenceAreaName { get; set; }
        public string ReportCurrencyIsoCode { get; set; }
        public DateTime ReportDate { get; set; }
        public string Resolution { get; set; }
        public string TemplateName { get; set; }
        public Boolean UseLiveCurrencyRates { get; set; }
		public CurvePriceType CurvePriceType { get; set; }
    }

    public class RelativePeriod
    {
        public int NumberOfPeriods { get; set; }
        public string PeriodKind { get; set; }
        public int FromDateOffsetFromReportDate { get; set; }
    }
}