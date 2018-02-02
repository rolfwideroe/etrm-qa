using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.HistoricMarketDataServiceReference;
using NUnit.Framework;
using TestWCFCurveService;

namespace TestWCFHistoricMarketDataService
{
    [TestFixture]
    public class GetHistoricExchangeRatesClass
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        private TestCase DeserializeXml(string testFilepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestCase));

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\HistoricExchangeRates\\" + testFilepath);

            FileStream readFileStream = File.Open(
                filepath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return (TestCase)xmlSerializer.Deserialize(readFileStream);
        }

        private static readonly IEnumerable<string> TestFileHistoricExchangeRates = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\HistoricExchangeRates");

        //used for test calculations
       // [Test, Timeout(1000*1000)]
        public void GetHistoricExchangeRatesForSomePeriod4Test()
        {

           
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();

            DateTime firstDate = new DateTime(2011, 11, 01, 0, 0, 0);
            DateTime lastDate = new DateTime(2011, 11, 02, 0, 0, 0);
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;

            KeyValuePair<DateTime, double>[] exchangeRates =
                service.GetHistoricExchangeRates("NOK" ,
                   "GBP", "Viz", firstDate, lastDate,
                   "Day", gapHandling);
            foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            {
                //Console.WriteLine("<ExpectedValue Date=\"" + s.Key.ToString("yyyy-MM-ddTHH:mm:ss") + "\" Value=\"" + s.Value + "\"/>");
               // Console.WriteLine(s.Key.ToString("yyyy-MM-ddTHH:mm:ss") + "\" Value=\"" + s.Value);
            }

            foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            {
                Console.Write(s.Value + "\n");

            }
        }

        [Test, Timeout(1000 * 1000), TestCaseSource("TestFileHistoricExchangeRates")]
        public void GetHistoricExchangeRates(string testFilepath)
        {

            TestCase test = this.DeserializeXml(testFilepath);
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();
            

            KeyValuePair<DateTime, double>[] exchangeRates = service.GetHistoricExchangeRates(test.InputData.BaseCurrencyISOCode,
                                                                                 test.InputData.CrossCurrencyCode, test.InputData.CurrencySource,
                                                                                 test.InputData.FirstDate, test.InputData.LastDate,
                                                                                 test.InputData.Resolution, test.InputData.GapHandling);
            //foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            //{
            //    Console.WriteLine("<ExpectedValue Date=\"" + s.Key.ToString("yyyy-MM-ddTHH:mm:ss") + "\" Value=\"" + s.Value + "\"/>");

            //}

            //foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            //{
            //    Console.Write(s.Value+"\n");

            //}
            

            for (int i = 0; i < test.ExpectedValues.Count(); i++)
            {
                DateTime actualDate = exchangeRates[i].Key;
                DateTime expectedDate = test.ExpectedValues[i].Date;

                string dateErrorMessage = "The actual Date : " + actualDate + " did not match the expected date : " +
                                          expectedDate + " at index = " + i;

                double actual = exchangeRates[i].Value;
                double expected = test.ExpectedValues[i].Value;

                if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                {
                    double relativeError = (actual / expected) - 1;
                    string errorMessage = "The actual Value for Date : " + actualDate + " did not match expected value " +
                                          actual + " vs " + expected;

                    Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE, errorMessage);
                }
                Assert.AreEqual(expectedDate, actualDate, dateErrorMessage);
            }

        }
        [Test]
        public void FailDateTimeKindisNotUnspecified()
        {

            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();

            DateTime firstDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Utc);
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;

            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "Viz", firstDate, new DateTime(2013, 12, 1), "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Variable 'FirstDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "Viz", new DateTime(2013, 02, 1), firstDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Variable 'LastDate' must be unspecified kind";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong Dates, firstDate > lastDate
        public void FailHistoricForFuture()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();

            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;

            DateTime firstDate = DateTime.Today.AddDays(1);
            DateTime unspFirstDate = DateTime.SpecifyKind(firstDate, DateTimeKind.Unspecified);

            DateTime lastDate = DateTime.Today.AddDays(31);
            DateTime unsplastDate = DateTime.SpecifyKind(lastDate, DateTimeKind.Unspecified);
           // Console.WriteLine(unspFirstDate + " / " + unsplastDate);

            //Console.WriteLine(DateTime.Today);
            try
            {
                KeyValuePair<DateTime, double>[] exchangeRate=service.GetHistoricExchangeRates("NOK", "USD", "Viz", unspFirstDate, unsplastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Cannot ask for historic exchange rates in the future";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }

            Assert.Fail("Expected to fail");
        }

        [Test] //wrong Dates, firstDate > lastDate
        public void FailFirstGreaterLast()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();

            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 6, 9, 0, 0, 0, DateTimeKind.Unspecified);// = sunday
            DateTime lastDate = new DateTime(2013, 2, 25, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "Viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Value of 'FirstDate' can't be greater than value of 'LastDate'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong CurrencySource
        public void FailCurrencySource()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();

            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Unspecified);
            DateTime lastDate = new DateTime(2013, 6, 25, 0, 0, 0, DateTimeKind.Unspecified);

            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "zzzzz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Currency source 'zzzzz' is not supported";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "", firstDate, lastDate, "Day", gapHandling);
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
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Unspecified);
            DateTime lastDate = new DateTime(2013, 6, 25, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "viz", firstDate, lastDate, "", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: Variable 'Resolution' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            //Supported resolutions: Hour, Day 
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "viz", firstDate, lastDate, "Year", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: Only hour and day is supported resolutions in GetHistoricExchangeRates";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            //Supported resolutions: Hour, Day.
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "viz", firstDate, lastDate, "Month", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: Only hour and day is supported resolutions in GetHistoricExchangeRates";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetHistoricExchangeRates("NOK", "USD", "viz", firstDate, lastDate, "zzzzz", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: Variable 'Resolution' has unknown value: 'zzzzz'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // wrong baseCurrencyISOCode
        public void FailBaseCurrencyCode()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 03, 31, 0, 0, 0, DateTimeKind.Unspecified);//=sunday
            DateTime lastDate = new DateTime(2013, 6, 25, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetHistoricExchangeRates("", "USD", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Variable 'FromCurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetHistoricExchangeRates("Nonecode", "USD", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // wrong crossCurrencyISOCode
        public void FailCrossCurrencyCode()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Unspecified);
            DateTime lastDate = new DateTime(2013, 6, 25, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetHistoricExchangeRates("NOK", "", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Variable 'ToCurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetHistoricExchangeRates("NOK", "Nonecode", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] // wrong baseCurrencyISOCode
        public void FailSameCurrencyCode()
        {
            IHistoricMarketDataService service = WCFClientUtil.GetHistoricMarketDateServiceProxy();
            GapHandling gapHandling = GapHandling.FillSmallGapsWithPreviousValue;
            DateTime firstDate = new DateTime(2013, 03, 31, 0, 0, 0, DateTimeKind.Unspecified);//=sunday
            DateTime lastDate = new DateTime(2013, 6, 25, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetHistoricExchangeRates("NOK", "NOK", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError = "Failed to return historic exchange rates: GetExchangeRates should not be called with from- and to-currency the same";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            try
            {
                service.GetHistoricExchangeRates("Nonecode", "USD", "viz", firstDate, lastDate, "Day", gapHandling);
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError =
                    "Failed to return historic exchange rates: GetExchangeRates should not be called with from- and to-currency the same";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                
            }
            Assert.Fail("Expected to fail");
        }

    }
}
