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

    public class CurveTestsGetForwardInterestRatesClass
    {

        private TestCase DeserializeXml(string testFilepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (TestCase));

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" +"InterestRates\\" + testFilepath);
            FileStream readFileStream = File.Open(
                filepath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return (TestCase) xmlSerializer.Deserialize(readFileStream);
        }
        [Test, Timeout(1000*1000), TestCaseSource(typeof (TestCasesFileEnumeratorInterest), "TestCaseFiles")]
        public void GetForwardInterestRates(string testFilepath)
        {
            TestCase test = this.DeserializeXml(testFilepath);
            ICurveService service = new CurveServiceClient();
            KeyValuePair<DateTime, double>[] interestRates = service.GetForwardInterestRates(test.InputData.CurrencyISOCode, test.InputData.CurrencySource, test.InputData.ReportDate, test.InputData.LastDate,
                                                               test.InputData.Resolution);
                        double floatingPointTollerance = 0.00001;
            for (int i = 0; i < test.ExpectedValues.Count(); i++)
            {
                DateTime actualDate = interestRates[i].Key;
                DateTime expectedDate = test.ExpectedValues[i].Date;

                string dateErrorMessage = "The actual Date : " + actualDate + " did not match the expected date : " +
                                          expectedDate + " at index = " + i;

                double actual = interestRates[i].Value;
                double expected = test.ExpectedValues[i].Value;

                if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                {
                    double relativeError = (actual / expected) - 1;
                    string errorMessage = "The actual Value for Date : " + actualDate + " did not match expcted value " +
                                          actual + " vs " + expected;
                    Assert.AreEqual(0, relativeError, floatingPointTollerance, errorMessage);
                }
                Assert.AreEqual(expectedDate,actualDate,dateErrorMessage);
            }

         }

        [Test] // DateTimeKind should be only Unspecified 
        public void FailDateTimeKindisNotUnspecified()
        {
            ICurveService service = new CurveServiceClient();
            DateTime reportDate=new DateTime(2013,5,5,0,0,0,DateTimeKind.Utc);
            try
            {
                service.GetForwardInterestRates("NOK", "Viz", reportDate, new DateTime(2013, 12, 1), "Day");
            }
            catch (Exception ex)
            {
                Assert.Pass();
            }
            Assert.Fail();

        }
        [Test] //Wrong currencySource. Supports only "viz"
        public void FailCurrencySource()
        {
            //check
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardInterestRates("NOK", "zzz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "week");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
               

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
            ICurveService service = new CurveServiceClient();
            try
            {
                service.GetForwardInterestRates("NOK", "viz", new DateTime(2013, 01, 1), new DateTime(2013, 12, 1), "SeaSun");
            }
           catch (FaultException<ExceptionDetail> ex)
           {
               const string ExpectedError =
                   "Failed to return forward interest rates: Resolution 'SeaSun' is not supported";
               Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
               return;
            }
            Assert.Fail("Expected to fail");
        }

        [Test] //wrong values: currency = ""; currency = "UA"
        public void FailCurrencyCode()
        {
            ICurveService service = new CurveServiceClient();
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
                const string ExpectedError ="Failed to return forward interest rates: Currency with iso code 'Nonecode' can't be found";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }
        
        [Test] //reportDate > lastDate
        public void FailReportDate()
        {
            ICurveService service = new CurveServiceClient();
            DateTime reportDate = new DateTime(2014, 6, 23, 0, 0, 0, DateTimeKind.Unspecified);
            try
            {
                service.GetForwardInterestRates("NOK", "viz", reportDate, new DateTime(2013, 07, 1), "Day");
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                const string ExpectedError ="Failed to return forward interest rates: Value of 'ReportDate' can't be greater than value of 'LastDate'";
                Assert.IsTrue(ex.Message.Contains(ExpectedError), "Wrong Error Message \n Expexted : " + ExpectedError + " \n Actual : " + ex.Message);
                return;
            }
            Assert.Fail("Expected to fail");
        }
    }
}
