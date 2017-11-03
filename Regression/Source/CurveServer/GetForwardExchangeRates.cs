using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;
using NUnit.Framework;
using TestWCFCurveService.CurveServiceReference;

namespace TestWCFCurveService
{
    [TestFixture]

    public class GetForwardExchangeRatesClass
    {
        private TestCase DeserializeXml(string testFilepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestCase));

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\ExchangeRates\\" + testFilepath);

            FileStream readFileStream = File.Open(
                filepath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return (TestCase)xmlSerializer.Deserialize(readFileStream);
        }
        
        [Test, Timeout(1000 * 1000), TestCaseSource(typeof(TestCasesFileEnumeratorExchange), "TestCaseFiles")]
        public void GetForwardExchangeRates(string testFilepath)
        {

            TestCase test = this.DeserializeXml(testFilepath);
            ICurveService service = new CurveServiceClient();
            KeyValuePair<DateTime, double>[] exchangeRates = service.GetForwardExchangeRates(test.InputData.BaseCurrencyISOCode,
                                                                                 test.InputData.CrossCurrencyCode,
                                                                                 test.InputData.CurrencySource, test.InputData.ReportDate,
                                                                                 test.InputData.LastDate, test.InputData.Resolution);
            //foreach (KeyValuePair<DateTime, double> s in exchangeRates)
            //{
            //    Console.WriteLine(s.Key + ";" + s.Value);
            //}
            double floatingPointTollerance = 0.00001;

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
                    string errorMessage = "The actual Value for Date : " + actualDate + " did not match expcted value " +
                                          actual + " vs " + expected;

                    Assert.AreEqual(0, relativeError, floatingPointTollerance, errorMessage);
                }
                Assert.AreEqual(expectedDate, actualDate, dateErrorMessage);
            }

        }
        [Test]
        public void FailDateTimeKindisNotUnspecified()
        {
            ICurveService service = new CurveServiceClient();
            DateTime reportDate = new DateTime(2013, 6, 5, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                service.GetForwardExchangeRates("NOK", "USD", "Viz", reportDate, new DateTime(2013, 12, 1), "Day");
            }
            catch (Exception ex)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }
          
        [Test] //wrong CurrencySource
        public void FailCurrencySource()
        {
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardExchangeRates("EUR","NOK", "z", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Failed to return forward exchange rates: Currency source 'z' is not supported";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("USD", "NOK", "", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Variable 'CurrencySource' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong resolution
        public void FailResolution()
        {
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardExchangeRates("EUR", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "SeaSun");
            }
           catch (FaultException<ExceptionDetail> ex)
           {
               const string ExpectedError =
                   "Failed to return forward exchange rates: Resolution 'SeaSun' is not supported";
               Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }
        [Test] // wrong baseCurrencyISOCode
        public void FailBaseCurrencyCode()
        {
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardExchangeRates("", "NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Failed to return forward exchange rates: Variable 'FromCurrencyISOCode' can't be null or empty";
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
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardExchangeRates("USD", "", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Failed to return forward exchange rates: Variable 'ToCurrencyISOCode' can't be null or empty";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
            }
            try
            {
                service.GetForwardExchangeRates("USD", "Nonecode", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Failed to return forward exchange rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }
        [Test] // reportDate = holyday
        public void FailDateSunday()
        {
            ICurveService service = new CurveServiceClient();
            try
            {                                                          //= sunday
                service.GetForwardExchangeRates("NOK", "USD", "ECB", new DateTime(2013, 06, 23), new DateTime(2013, 07, 05), "Day");
            }
            catch (Exception ex)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

    }
}
