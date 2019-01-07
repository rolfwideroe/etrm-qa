namespace ElvizTestUtils.CurveTests
{
    public class CurveExpectedValues
    {
        public string ErrorMessage { get; set; }
        public ExpectedCurveProperties ExpectedProperties { get; set; }
        public ExpectedCurveValue[] ExpectedCurveValues { get; set; }
        public double? ExpectedRepeatingCurveValue { get; set; }
        public ExpectedCurveVolatility ExpectedCurveVolatility { get; set; }
    }
}