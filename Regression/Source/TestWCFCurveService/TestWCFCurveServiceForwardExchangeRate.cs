using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using NUnit.Framework;

namespace TestWCFCurveService
{
    [TestFixture]

    public class TestWCFCurveServiceForwardExchangeRate
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TearDown]
        public void TearDown()
        {
            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
        }

        private const string ForwarExchangeRatesFilePath = "TestFiles\\ExchangeRates\\";

        private static readonly IEnumerable<string> TestFilesForwardExchangeRates = TestCasesFileEnumeratorByFolder.TestCaseFiles(ForwarExchangeRatesFilePath);

        [Test, Timeout(1000*150), TestCaseSource("TestFilesForwardExchangeRates")]
        public void GetForwardExchangeRates(string testFile)
        {


            string testFilepath = Path.Combine(Directory.GetCurrentDirectory(), ForwarExchangeRatesFilePath + testFile);

            ExchangeRateTestCase rateTestCase = TestXmlTool.Deserialize<ExchangeRateTestCase>(testFilepath);

            KeyValuePair<DateTime, double>[] exchangeRates = ExchangeRateTestUtil.GetExchangeRates(rateTestCase);

            //ouput of results
            //foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            //{
            //    Console.WriteLine("<ExpectedCurveValue Date=\"" + s.Key.ToString("yyyy-MM-ddTHH:mm:ss") +
            //                      "\" Value=\"" + s.Value + "\"/>");
            //}

            Assert.IsTrue(rateTestCase.ExpectedCurveValues.Any(), "Test case does not contain expected values");
            Assert.AreEqual(rateTestCase.ExpectedCurveValues.Length, exchangeRates.Length, "Wrong number of records");

            for (int i = 0; i < rateTestCase.ExpectedCurveValues.Count(); i++)
            {
                DateTime actualDate = exchangeRates[i].Key;
                DateTime expectedDate = rateTestCase.ExpectedCurveValues[i].Date;

                string dateErrorMessage = "The actual Date : " + actualDate + " did not match the expected date : " +
                                          expectedDate + " at index = " + i;

                double actual = exchangeRates[i].Value;
                double expected = rateTestCase.ExpectedCurveValues[i].Value;

                if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                {
                    double relativeError = (actual/expected) - 1;
                    string errorMessage = "The actual Value for Date : " + actualDate + " did not match expected value " +
                                          actual + " vs " + expected;

                    Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,
                        errorMessage);
                }
                Assert.AreEqual(expectedDate, actualDate, dateErrorMessage);
            }

        }


        [Test]
        public void FailDateTimeKindisNotUnspecified()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            DateTime reportDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                service.GetForwardExchangeRates("NOK", "USD", "Viz", reportDate, new DateTime(2013, 12, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Variable 'ReportDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            DateTime lastDate = new DateTime(2013, 5, 5, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                service.GetForwardExchangeRates("NOK", "NOK", "Viz", new DateTime(2013, 02, 01), lastDate, "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Variable 'LastDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong CurrencySource
        public void FailCurrencySource()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardExchangeRates("EUR", "NOK", "z", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: The currencySource parameter must be set to the forward currency source configured in the system (Viz)";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("USD", "NOK", "", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Variable 'CurrencySource' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong resolution
        public void FailResolution()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardExchangeRates("EUR", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "SeaSun");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward exchange rates: Variable 'Resolution' has unknown value: 'SeaSun'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("EUR", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward exchange rates: Variable 'Resolution' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // wrong baseCurrencyISOCode
        public void FailBaseCurrencyCode()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardExchangeRates("", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Variable 'FromCurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("Nonecode", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward exchange rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // wrong crossCurrencyISOCode
        public void FailCrossCurrencyCode()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardExchangeRates("USD", "", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Variable 'ToCurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("USD", "Nonecode", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // reportDate = holiday
        public void ExchangeRatePassDateSunday()
        {
			ElvizConfiguration[] ecbForwardExchangeSourceConfiguration = new[] { new ElvizConfiguration("CurrencySource", "ECB") };
	        try
	        {
                ElvizConfigurationTool utility = new ElvizConfigurationTool();
				utility.UpdateConfiguration(ecbForwardExchangeSourceConfiguration);

		        ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
		        try
		        {
			        //= sunday = holiday
			        service.GetForwardExchangeRates("NOK", "USD", "ECB", new DateTime(2011, 04, 17), new DateTime(2014, 07, 09),
				        "Day");
		        }
		        catch (FaultException<ExceptionDetail> ex)
		        {
			        Assert.Fail("Test should be successfully pass. Error: " + ex);
			        return;
		        }
		        Assert.Pass();
	        }

	        finally
	        {
                ElvizConfigurationTool utility = new ElvizConfigurationTool();
				utility.RevertAllConfigurationsToDefault();
	        }
        }

	    [Test] //reportDate > lastDate
        public void FailReportDate()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            DateTime reportDate = new DateTime(2014, 6, 23, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetForwardExchangeRates("NOK", "NOK", "viz", reportDate, new DateTime(2013, 07, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward exchange rates: Value of 'ReportDate' can't be greater than value of 'LastDate'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }
    }
}
