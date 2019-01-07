using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Apache.NMS;
using Apache.NMS.Util;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestWCFGetPriceCurveByCriteriaLive
{
    
    [TestFixture]
    public class TestWCFGetPriceCurveByCriteriaLive
    {
        private const string FXSpotPath = "Testfiles\\FXSpot\\SPOT.xml";

        [OneTimeSetUp]
        public void Setup()
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), FXSpotPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            SendToQueue(doc.InnerXml);
            Thread.Sleep(200);
            RunJob("LiveFXEl (Close Price)");
            Thread.Sleep(200);
            RunJob("LiveFXGas (Close Price)");
        }

        public static void RunJob(string description)
        {
            //running job for publishing pricebook
            const string curveJobType = "Live Price Book Snapshot Job";
            int curveJobId = JobAPI.GetJobsIdByDescription(description, curveJobType);
            JobAPI.ExecuteAndAssertJob(curveJobId, 10);
        }

        //sending spot fx rates
        public void SendToQueue(string docInnerXml)
        {
            string appServerName = ElvizInstallationUtility.GetAppServerName();
            string servername = "tcp://" + appServerName + ":61616/";

            Uri connecturi = new Uri(servername);
            // Console.WriteLine("About to connect to " + connecturi);
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            try
            {
                using (IConnection connection = factory.CreateConnection())
                using (ISession session = connection.CreateSession())
                {
                    IDestination destination = SessionUtil.GetDestination(session, "queue://PBCustomFeedInputQueue");
                    // Console.WriteLine("Using destination: " + destination);
                    // Create a consumer and producer
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        connection.Start();
                        producer.DeliveryMode = MsgDeliveryMode.Persistent;
                        ITextMessage request = session.CreateTextMessage(docInnerXml);
                        //Console.WriteLine(docInnerXml);
                        request.NMSType = "Viz.Priceboard.Adapter.Interface.PriceFeed";
                        producer.Send(request);
                    }
                }
            }
            catch (Exception exception)
            {
                {
                    Console.WriteLine(exception.Message);
                }
                throw;
            }

        }


        private const string PriceCurveFilePath = "Testfiles\\";

        private static readonly IEnumerable<string> PriceCurveLive = TestCasesFileEnumeratorByFolder.TestCaseFiles(PriceCurveFilePath);

        [Test, Timeout(1000 * 1000), TestCaseSource("PriceCurveLive")]
        public void TestGetPriceCurveByCriteriaLive(string testFile)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), PriceCurveFilePath + testFile);
          
            TestPriceCurveDto(filepath);
        }
        
        private void TestPriceCurveDto(string testFilePath)
        {
            CurveTestCase curveTest = TestXmlTool.Deserialize<CurveTestCase>(testFilePath);

            DateTime reportDate = curveTest.InputData.ReportDate;
            if (reportDate == DateTime.MinValue) reportDate = new DateTime(DateTime.Now.Date.Ticks, DateTimeKind.Unspecified);

            DateTime? fromDate = curveTest.InputData.FromDate;
            DateTime? toDate = curveTest.InputData.ToDate;
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                RelativePeriod relativePeriod = curveTest.InputData.RelativePeriod;
                Assert.IsNotNull(relativePeriod);

                switch (relativePeriod.PeriodKind)
                {
                    case "Day":
                        fromDate = reportDate.AddDays(relativePeriod.FromDateOffsetFromReportDate);
                        toDate = fromDate.Value.AddDays(relativePeriod.NumberOfPeriods).AddDays(-1);
                        break;
                    case "Month":
                        fromDate = new DateTime(reportDate.Year, reportDate.Month, 1).AddMonths(relativePeriod.FromDateOffsetFromReportDate);
                        toDate = fromDate.Value.AddMonths(relativePeriod.NumberOfPeriods).AddDays(-1);
                        break;
                    case "Year":
                        fromDate = new DateTime(reportDate.Year, 1, 1).AddYears(relativePeriod.FromDateOffsetFromReportDate);
                        toDate = fromDate.Value.AddYears(relativePeriod.NumberOfPeriods).AddDays(-1);
                        break;
                    default:
                        throw new NotSupportedException($"RelativePeriod.PeriodKind = {relativePeriod.PeriodKind} is not supported.");
                }
            }

            ElvizConfiguration[] elvizConfigurations = curveTest.InputData.ElvizConfigurations;
            try
            {
                if (elvizConfigurations != null)
                {
                    ElvizConfigurationTool utility = new ElvizConfigurationTool();

                    utility.UpdateConfiguration(elvizConfigurations);
                }

                ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
                PriceCurveCriteria criteria = new PriceCurveCriteria
                {
                    CurrencyQuote = curveTest.InputData.CurrencyQuote,
                    FromDate = fromDate,
                    ToDate = toDate,
                    LoadType = curveTest.InputData.LoadType,
                    PriceBookAppendix = curveTest.InputData.PriceBookAppendix,
                    ReferenceAreaName = curveTest.InputData.ReferenceAreaName,
                    ReportCurrencyIsoCode = curveTest.InputData.ReportCurrencyIsoCode,
                    ReportDate = reportDate,
                    Resolution = curveTest.InputData.Resolution,
                    TemplateName = curveTest.InputData.TemplateName,
                    UseLiveCurrencyRates = curveTest.InputData.UseLiveCurrencyRates,
                    CurvePriceType = curveTest.InputData.CurvePriceType
                };


                PriceCurveDto testCurveDto = null;
                try
                {
                    testCurveDto = service.GetPriceCurveByCriteria(criteria);
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(curveTest.ExpectedValues.ErrorMessage))
                    {
                        Console.WriteLine(ex.Message);
                        Assert.AreEqual(curveTest.ExpectedValues.ErrorMessage, ex.Message, "Test Expected to Fail but other Exception was caught: " + ex.Message);
                    }
                    else throw;
                }

                if (testCurveDto != null)
                {
                    QaPriceCurveDtoWrapper wrapper = new QaPriceCurveDtoWrapper(testCurveDto);

                    //Addititonal information, get number values from percent value
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterShort,
                        wrapper.PriceCurveVolatility.WinterShort, "Volatilty : WinterShort not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterMedium,
                        wrapper.PriceCurveVolatility.WinterMedium, "Volatilty : WinterMedium not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterLong,
                        wrapper.PriceCurveVolatility.WinterLong, "Volatilty : WinterLong not equal");
                    Assert.AreEqual(
                        curveTest.ExpectedValues.ExpectedCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium,
                        wrapper.PriceCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium,
                        "Volatilty : NumberOfYears not equal");

                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerShort,
                        wrapper.PriceCurveVolatility.SummerShort, "Volatilty : SummerShort not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerMedium,
                        wrapper.PriceCurveVolatility.SummerMedium, "Volatilty : SummerMedium not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerLong,
                        wrapper.PriceCurveVolatility.SummerLong, "Volatilty : SummerLong not equal");
                    Assert.AreEqual(
                        curveTest.ExpectedValues.ExpectedCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium,
                        wrapper.PriceCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium,
                        "Volatilty : NumberOfYears not equal");


                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedProperties.CurveType, wrapper.CurveType,
                        "CurveType not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedProperties.VolumeUnit, wrapper.VolumeUnit,
                        "VolumeUnit not equal");
                    Assert.AreEqual(curveTest.ExpectedValues.ExpectedProperties.TimeZone, wrapper.TimeZone,
                        "TimeZone not equal");


                    List<DateTimeValue> expectedTimeValues;
                    if (curveTest.ExpectedValues.ExpectedRepeatingCurveValue.HasValue && curveTest.ExpectedValues.ExpectedCurveValues == null)
                    {
                        double expectedValue = curveTest.ExpectedValues.ExpectedRepeatingCurveValue.Value;
                        expectedTimeValues = new List<DateTimeValue>();

                        DateTime dateTime = criteria.FromDate.Value;
                        do
                        {
                            expectedTimeValues.Add(new DateTimeValue(dateTime, expectedValue));
                            switch (criteria.Resolution)
                            {
                                case "15 min":
                                    dateTime = dateTime.AddMinutes(15);
                                    break;
                                case "30 min":
                                    dateTime = dateTime.AddMinutes(30);
                                    break;
                                case "Day":
                                    dateTime = dateTime.AddDays(1);
                                    break;
                                case "Month":
                                    dateTime = dateTime.AddMonths(1);
                                    break;
                                case "Year":
                                    dateTime = dateTime.AddYears(1);
                                    break;
                                default:
                                    throw new NotSupportedException($"InputData.Resolution = {criteria.Resolution} is not supported for ExpectedRepeatingCurveValue.");
                            }
                            
                        } while (dateTime < criteria.ToDate.Value.AddDays(1));
                    }
                    else
                    {
                        expectedTimeValues = curveTest.ExpectedValues.ExpectedCurveValues.Select(d => new DateTimeValue(d.Date, d.Value)).ToList();
                    }

                    List<DateTimeValue> dateTimeValues = wrapper.GetCurveValues();
/*
                    foreach (DateTimeValue record in dateTimevalues)
                    {
                        string s = "";
                       // s += record.DateTime + " ; " + record.Value;
                        s += "<ExpectedCurveValue Date=\"" + record.DateTime.ToString("yyyy-MM-ddTHH:mm:ss") +
                             "\" Value=\"" + record.Value + "\"/>";
                          // Console.WriteLine(s);
                    }
*/

                    Assert.AreEqual(expectedTimeValues.Count, dateTimeValues.Count, "Test case contains wrong number of expected values");

                    for (int i = 0; i < expectedTimeValues.Count; i++)
                    {
                        //Assert Data
                        Assert.AreEqual(expectedTimeValues[i].DateTime, dateTimeValues[i].DateTime,
                            "The actual Date : " + dateTimeValues[i].DateTime + " did not match the expected date : " +
                            expectedTimeValues[i].DateTime + " at index = " + i);

                        double actual = dateTimeValues[i].Value;
                        double expected = expectedTimeValues[i].Value;
                       
                        if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                        {
                            double relativeError = (actual / expected) - 1;
                            string errorMessage = "Failed for record : " + dateTimeValues[i].DateTime +
                                                  " : Expected value is : " +
                                                  expected + " , but Actual value was " + actual;
                            //Assert value
                            Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,
                                errorMessage);
                        }
                    }

                    Assert.AreEqual(curveTest.InputData.LoadType, testCurveDto.LoadType, "LoadType not equal");
                    Assert.AreEqual(curveTest.InputData.PriceBookAppendix, testCurveDto.PriceBookAppendix,
                        "PriceBookAppendix not equal");

                    if (!testCurveDto.IsIndexPriceCurve)
                        Assert.AreEqual(curveTest.InputData.ReferenceAreaName, testCurveDto.ReferenceAreaName,
                            "ReferenceArea not equal");

                    Assert.AreEqual(curveTest.InputData.ReportCurrencyIsoCode, testCurveDto.Currency,
                        "Currency not equal");
                    Assert.AreEqual(reportDate, testCurveDto.ReportDate, "ReportDate not equal");
                    Assert.AreEqual(curveTest.InputData.Resolution, testCurveDto.PriceSeriesResolution,
                        "Resolution not equal");
                    Assert.AreEqual(curveTest.InputData.TemplateName, testCurveDto.PriceBookName,
                        "TemplateName not equal");
                }
            }
            finally
            {
                if (elvizConfigurations != null)
                {
                    ElvizConfigurationTool utility = new ElvizConfigurationTool();
                    utility.RevertAllConfigurationsToDefault();
                }
            }
        }

        [Test]
        public void JobExecutionWithOptionalParams()
        {
            double expectedPrice250716 = 27.380;
            int jobId = 15;
            string date = "20160725"; // run for report date
            Dictionary<string, string> optionalParams = new Dictionary<string, string> {{"RunForDate", date}};
            JobAPI.ExecuteAndAssertJob(jobId, optionalParams, 30);

            //getting prices
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                CurrencyQuote = null,
                FromDate = new DateTime(2016, 07, 25),
                ToDate = new DateTime(2016, 07, 25),
                LoadType = "Base",
                PriceBookAppendix = null,
                ReferenceAreaName = "NPS",
                ReportCurrencyIsoCode = "EUR",
                ReportDate = new DateTime(2016, 07, 22), // date(25.07) is Monday, so function will create end of yesterday pricebook for Friday before (22.07).
                Resolution = "Day",
                TemplateName = "NPS_25072016",
                UseLiveCurrencyRates = false
            };
            
            try
            {
                PriceCurveDto testCurveDto = service.GetPriceCurveByCriteria(criteria);
                Assert.AreEqual(1, testCurveDto.Prices.Length, "Expected price curve length are not equal actual.");
                double relativeError = (testCurveDto.Prices[0] / expectedPrice250716) - 1;
                string errorMessage = "The actual Price for Date '" + date + "' did not match expected value " +
                                      expectedPrice250716 + " vs " +testCurveDto.Prices[0] ;

                Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE, errorMessage);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
