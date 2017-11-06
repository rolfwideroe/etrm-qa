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

    public class TestWCFCurveServiceForwardInterestRates
    {

      

        private const string ForwarInterestRateFilePath = "TestFiles\\InterestRates\\";

        private static readonly IEnumerable<string> TestFilesInterestRates = TestCasesFileEnumeratorByFolder.TestCaseFiles(ForwarInterestRateFilePath);

        [Test, Timeout(1000 * 10), TestCaseSource("TestFilesInterestRates")]
        public void GetForwardInterestRates(string testFile)
        {
            string testFilepath = Path.Combine(Directory.GetCurrentDirectory(), ForwarInterestRateFilePath + testFile);

            InterestRateTestCase interestRateTestCase = TestXmlTool.Deserialize<InterestRateTestCase>(testFilepath);

            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();

            KeyValuePair<DateTime, double>[] interestRates = service.GetForwardInterestRates(interestRateTestCase.InputData.CurrencyISOCode, interestRateTestCase.InputData.CurrencySource, interestRateTestCase.InputData.ReportDate, interestRateTestCase.InputData.LastDate,
                                                               interestRateTestCase.InputData.Resolution);
            //foreach (KeyValuePair<DateTime, double> s in interestRates)
            //{
            //  Console.WriteLine("<ExpectedValue Date=\"" + s.Key.ToString("yyyy-MM-ddTHH:mm:ss") + "\" FeeValue=\"" + s.FeeValue + "\"/>");
            //}

           //  double floatingPointTollerance = 0.00001;

            Assert.IsTrue(interestRateTestCase.ExpectedCurveValues.Count() > 0, "Test case does not contain expected values");

            for (int i = 0; i < interestRateTestCase.ExpectedCurveValues.Count(); i++)
            {
                DateTime actualDate = interestRates[i].Key;
                DateTime expectedDate = interestRateTestCase.ExpectedCurveValues[i].Date;
                string dateErrorMessage = "The actual Date : " + actualDate + " did not match the expected date : " +
                                          expectedDate + " at index = " + i;

                double actual = interestRates[i].Value;
                double expected = interestRateTestCase.ExpectedCurveValues[i].Value;

                if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                {
                    double relativeError = (actual / expected) - 1;
                    string errorMessage = "The actual FeeValue for Date : " + actualDate + " did not match expected value " +
                                          actual + " vs " + expected;
                    Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE, errorMessage);
                }
                Assert.AreEqual(expectedDate, actualDate, dateErrorMessage);
            }

        }

        [Test] // DateTimeKind should be only Unspecified 
        public void FailDateTimeKindisNotUnspecified()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            DateTime reportDate = new DateTime(2013, 5, 5, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                service.GetForwardInterestRates("NOK", "Viz", reportDate, new DateTime(2013, 12, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward interest rates: Variable 'ReportDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            DateTime lastDate = new DateTime(2013, 5, 5, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                service.GetForwardInterestRates("NOK", "Viz", new DateTime(2013, 02, 01), lastDate, "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward interest rates: Variable 'LastDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");

        }

        [Test] //Wrong currencySource. Supports only "viz"
        public void FailCurrencySource()
        {
            //check
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardInterestRates("NOK", "zzz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Forward interest rates supports only 'Viz' currency source at the moment";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardInterestRates("NOK", "", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
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
                service.GetForwardInterestRates("NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "SeaSun");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward interest rates: Variable 'Resolution' has unknown value: 'SeaSun'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardInterestRates("NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward interest rates: Variable 'Resolution' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong values: currency = ""; currency = "UA"
        public void FailCurrencyCode()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {
                service.GetForwardInterestRates("", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return forward interest rates: Variable 'CurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardInterestRates("Nonecode", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward interest rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //reportDate > lastDate
        public void FailReportDate()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            DateTime reportDate = new DateTime(2014, 6, 23, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetForwardInterestRates("NOK", "viz", reportDate, new DateTime(2013, 07, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return forward interest rates: Value of 'ReportDate' can't be greater than value of 'LastDate'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // reportDate = holiday
        public void PassDateSunday()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            try
            {                                                          //= sunday = holiday
                service.GetForwardInterestRates("NOK", "Viz", new DateTime(2013, 03, 31), new DateTime(2013, 07, 09), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                Assert.Fail("Test should be successfully pass. Error: " + ex);
                return;
            }
            Assert.Pass();
        }
    }
}
