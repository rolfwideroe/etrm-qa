using System;

namespace ElvizTestUtils.CurveTests
{
    public class ExchangeRateInputData
    {
        public ElvizConfiguration[] ElvizConfigurations { get; set; }
        public string BaseCurrencyISOCode { get; set; }
        public string CrossCurrencyISOCode { get; set; }
        public string CurrencySource { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime LastDate { get; set; }
        public string Resolution { get; set; }
        
    }
}