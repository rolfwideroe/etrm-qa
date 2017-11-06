using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Apache.NMS;
using Apache.NMS.Util;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.QaLookUp;
using ElvizTestUtils.ReportEngineTests;
using NUnit.Framework;

namespace TestWCFGetPriceCurveByCriteriaLive
{
    
    [TestFixture]
    public class TestWCFGetPriceCurveByCriteriaLive
    {
        private const string FXSpotPath = "Testfiles\\FXSpot\\SPOT.xml";

        [TestFixtureSetUp]
        public void Setup()
        {

            string filename = Path.Combine(Directory.GetCurrentDirectory(), FXSpotPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            this.SendToQueue(doc.InnerXml);
            Thread.Sleep(200);
            RunJob("LiveFXEl (Close Price)");
            Thread.Sleep(200);
            RunJob("LiveFXGas (Close Price)");
        }


        public static void RunJob(string description)
            {
                //running job for publishing pricebook
                string curveJobType = "Live Price Book Snapshot Job";
                int curveJobId = JobAPI.GetJobsIdByDescription(description, curveJobType);
                JobAPI.ExecuteAndAssertJob(curveJobId,10);
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
            string dateformat = "yyyy-MM-dd";
            string time = DateTime.Now.ToString(dateformat); 

            if (reportDate == DateTime.MinValue) reportDate = DateTime.Parse(time);

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
                    FromDate = curveTest.InputData.FromDate,
                    ToDate = curveTest.InputData.ToDate,
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


                if (!string.IsNullOrEmpty(curveTest.ExpectedValues.ErrorMessage))
                {
                    try
                    {
                        PriceCurveDto testCurveDtoFail = service.GetPriceCurveByCriteria(criteria);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Assert.AreEqual(curveTest.ExpectedValues.ErrorMessage, ex.Message,
                            "Test Expected to Fail but other Exception was caught: " + ex.Message);

                    }
                }
                else
                {
                    PriceCurveDto testCurveDto = service.GetPriceCurveByCriteria(criteria);
                    QaPriceCurveDtoWrapper wrapper =
                        new QaPriceCurveDtoWrapper(service.GetPriceCurveByCriteria(criteria));


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

                    List<DateTimeValue> dateTimevalues = wrapper.GetCurveValues();

                    List<DateTimeValue> expectedTimevalues =
                        curveTest.ExpectedValues.ExpectedCurveValues.Select(d => new DateTimeValue(d.Date, d.Value))
                            .ToList();

                    foreach (DateTimeValue record in wrapper.GetCurveValues())
                    {
                        string s = "";
                       // s += record.DateTime + " ; " + record.Value;
                        s += "<ExpectedCurveValue Date=\"" + record.DateTime.ToString("yyyy-MM-ddTHH:mm:ss") +
                             "\" Value=\"" + record.Value + "\"/>";

                          // Console.WriteLine(s);
                    }

                    Assert.AreEqual(expectedTimevalues.Count, dateTimevalues.Count,
                        "Test case contains wrong number of expected values");

                    for (int i = 0; i < expectedTimevalues.Count; i++)
                    {
                        //Assert Data
                        Assert.AreEqual(expectedTimevalues[i].DateTime, dateTimevalues[i].DateTime,
                            "The actual Date : " + dateTimevalues[i].DateTime + " did not match the expected date : " +
                            expectedTimevalues[i].DateTime + " at index = " + i);

                        double actual = dateTimevalues[i].Value;
                        double expected = expectedTimevalues[i].Value;
                       
                        if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                        {
                            double relativeError = (actual / expected) - 1;
                            string errorMessage = "Failed for record : " + dateTimevalues[i].DateTime +
                                                  " : Expected value is : " +
                                                  expected + " , but Actual value was " + actual;
                            //Assert value
                            Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,
                                errorMessage);
                        }
                    }

                    //    if(curveTest.ResultInformation.ResultValue.ToUpper()=="EXCEPTION") Assert.Fail("Test Passed, but expected to fail with error :\n "+curveTest.ResultInformation.ErrorMessage);

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
