using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElvizTestUtils.CurveTests
{
    public class InterestRateTestCase
    {
        public InterestRateInputData InputData { get; set; }
        public ExpectedCurveValue[] ExpectedCurveValues { get; set; }
    }

    public class InterestRateInputData
    {
        public string CurrencyISOCode { get; set; }
        public string CurrencySource { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime LastDate { get; set; }
        public string Resolution { get; set; }
    }
}
