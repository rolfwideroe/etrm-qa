using System;
using System.Collections.Generic;
using ElvizTestUtils.CurveServiceReference;

namespace ElvizTestUtils.CurveTests
{
    public class ExchangeRateTestUtil
    {


        public static KeyValuePair<DateTime, double>[] GetExchangeRates(ExchangeRateTestCase exchangeRateTestCase)
        {
                ElvizConfiguration[] elvizConfigurations = exchangeRateTestCase.InputData.ElvizConfigurations;
           

                if (elvizConfigurations == null) throw new ArgumentException("Test Expected to contain Configurations");
                

                    ElvizConfigurationTool utility = new ElvizConfigurationTool();

                    utility.UpdateConfiguration(elvizConfigurations);
                


                ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
                KeyValuePair<DateTime, double>[] exchangeRates =
                    service.GetForwardExchangeRates(exchangeRateTestCase.InputData.BaseCurrencyISOCode,
                        exchangeRateTestCase.InputData.CrossCurrencyISOCode,
                        exchangeRateTestCase.InputData.CurrencySource, exchangeRateTestCase.InputData.ReportDate,
                        exchangeRateTestCase.InputData.LastDate, exchangeRateTestCase.InputData.Resolution);


            return exchangeRates;

        }
    }
}
