using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.InternalJobService;
using NUnit.Framework;


namespace TestLiveCurveScheduling
{
    class TestWCFGetPriceCurveByCriteria
    {
           //send ActiveMQ request with prices to PriceBook
           
            private static readonly IEnumerable<string> TestFilesLiveCurve = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");    
            [Test, Timeout(1000 * 1000), TestCaseSource("TestFilesLiveCurve")]
            public static void ProcessQuery(string testFilepath) 
            {
                
                string filename = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFilepath);

                LiveCurveSchedulingTestCase testCase = TestXmlTool.Deserialize<LiveCurveSchedulingTestCase>(filename);

                PriceBoardCustomPriceFeed feed= new PriceBoardCustomPriceFeed();
                feed.Parameters = new PriceBoardCustomPriceFeedParameters()
                   {
                       Commodity = testCase.Commodity,
                       Couterparty = testCase.Couterparty,
                       Feed = testCase.Feed,
                       IsOption = testCase.IsOption,
                       IsSwap = testCase.IsSwap,
                       RefArea = testCase.RefArea,
                       TimeZone = testCase.TimeZone,
                       Venue = testCase.Venue
                    };

                    PriceBoardCustomPriceFeedPrice[] prices = new PriceBoardCustomPriceFeedPrice[testCase.LivePrices.Length];

                    for (int i = 0; i < testCase.LivePrices.Length; i++)
                    {
                        LivePrice l = testCase.LivePrices[i];
                        PriceBoardCustomPriceFeedPrice p = new PriceBoardCustomPriceFeedPrice()
                        {
                            PeriodType = l.PeriodType,
                            Ask = l.Ask,
                            Bid = l.Bid,
                            Close = l.Close,
                            Last = l.Last,
                            LoadType = l.LoadType,
                            From = l.FromDateTime,
                            To=l.ToDateTime,
                            Ticker = l.Ticker
                        };

                        prices[i] = p;
                    }  
                    feed.Prices = prices;

                //creating request and sending it to queue
                string s = TestXmlTool.Serialize(feed);
               // Console.WriteLine(s);
                SendToQueue(s);

                //running job for publishing pricebook
                string curveJobType = "Live Price Book Snapshot Job";
                int curveJobId = JobAPI.GetJobsIdByDescription(testCase.Feed, curveJobType);
                JobAPI.ExecuteAndAssertJob(curveJobId,10);

                //getting prices from live pricebook
                GetPriceCurveByCriteria(testCase);
            }


      //  [Test]
        public void TickerTest()
        {
          //  string ticker = "ELCEUR{MMM-yy}";
            string ticker2 = "{MMM-yy}";

         //   IList<string> parts = ticker.Split();
            
            DateTime date=new DateTime(2018,03,18);

          //  TickerParser.ParseCustomPeriod(ticker, date);
          Console.WriteLine(TickerParser.Parse(ticker2,"Month",1));

           // Console.WriteLine(TickerParser.Parse(ticker2,"quarter",1));
        }

        public static void SendToQueue(string docInnerXml)
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
       
            //Get prices from PriceBook      
            public static void GetPriceCurveByCriteria(LiveCurveSchedulingTestCase testCase)
            {
               
                ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
                
                string templateName = testCase.Feed;
                string refArea = testCase.RefArea;
                string currencyCode = testCase.Currency;

                for (int i = 0; i < testCase.LivePrices.Length; i++)
                {
                    LivePrice l = testCase.LivePrices[i];
                    string periodType = l.PeriodType;
                    //El.Cert and Emission resolution should be day instead of workday and custom period 
                    if (periodType.ToUpper() == "WORKDAY" || periodType.ToUpper() == "CUSTOMPERIOD")
                        periodType = "Day";
                 
                    PriceCurveCriteria criteria = new PriceCurveCriteria
                   {
                                FromDate = l.FromDateTime,
                                ToDate = l.ToDateTime,
                                LoadType = l.LoadType,
                                PriceBookAppendix = string.Empty,
                                ReferenceAreaName = refArea,
                                ReportCurrencyIsoCode = currencyCode,
                                ReportDate = Convert.ToDateTime(DateTime.Today.ToString("d")),
                                Resolution = periodType,
                                TemplateName = templateName
                    };
                   //getting prices
                    try
                            {
                                PriceCurveDto testCurveDto = service.GetPriceCurveByCriteria(criteria);
                                //for (int i = 0; i< 100; i++)
                                //{
                                //    Console.WriteLine(testCurveDto.Prices[i]);
                                //}
                              //  Console.WriteLine("For period= " + fromDate.ToString("d") + " - " + toDate.ToString("d") + " price =" + testCurveDto.Prices[0]);
                                double actualPrice = testCurveDto.Prices[0];
                                if (Math.Abs(actualPrice) >= 1e-8 || Math.Abs(l.Close) >= 1e-8)
                                {
                                    double relativeError = (actualPrice / l.Close) - 1;
                                    string errorMessage = "The actual Value for period : " + l.FromDateTime.ToString("d") + " - " + l.ToDateTime.ToString("d") + " did not match expected value " +
                                                          actualPrice + " vs " + l.Close;
                                    //Assert value
                                    Assert.AreEqual(0, relativeError, 1e-7, errorMessage);
                                }

                                Assert.AreEqual(l.LoadType, testCurveDto.LoadType, "LoadType not equal");
                                Assert.AreEqual(criteria.ReportCurrencyIsoCode, testCurveDto.Currency, "Currency not equal");
                                Assert.AreEqual(criteria.ReportDate, testCurveDto.ReportDate, "ReportDate not equal");
                                Assert.AreEqual(periodType, testCurveDto.PriceSeriesResolution, "Resolution not equal");
                                Assert.AreEqual(templateName, testCurveDto.PriceBookName, "TemplateName not equal");
                            }
                            catch (Exception ex)
                            {
                                Assert.Fail("Expected To Pass, but Failed with Exception : " + ex.Message);
                                return;
                            }
                     }
            }

      
    }

}