using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.QaLookUp;

namespace ElvizTestUtils.CurveTests
{
    public class CurveTestUtil
    {


        public static QaPriceCurveDtoWrapper GetCurve(CurveTestCase curveTestCase)
        {
            ElvizConfiguration[] elvizConfigurations = curveTestCase.InputData.ElvizConfigurations;

            if (elvizConfigurations != null)
            {
                ElvizConfigurationTool tool = new ElvizConfigurationTool();

                tool.UpdateConfiguration(elvizConfigurations);
            }

            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                FromDate = curveTestCase.InputData.FromDate,
                ToDate = curveTestCase.InputData.ToDate,
                LoadType = curveTestCase.InputData.LoadType,
                PriceBookAppendix = curveTestCase.InputData.PriceBookAppendix,
                ReferenceAreaName = curveTestCase.InputData.ReferenceAreaName,
                ReportCurrencyIsoCode = curveTestCase.InputData.ReportCurrencyIsoCode,
                ReportDate = curveTestCase.InputData.ReportDate,
                Resolution = curveTestCase.InputData.Resolution,
                TemplateName = curveTestCase.InputData.TemplateName,
                CurrencyQuote = curveTestCase.InputData.CurrencyQuote,
                UseLiveCurrencyRates = curveTestCase.InputData.UseLiveCurrencyRates,
				CurvePriceType = curveTestCase.InputData.CurvePriceType
            };

            return new QaPriceCurveDtoWrapper(service.GetPriceCurveByCriteria(criteria));

        }
    }
}
