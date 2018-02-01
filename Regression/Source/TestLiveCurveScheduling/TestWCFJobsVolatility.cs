using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.DatabaseTools;
using NUnit.Framework;

namespace TestLiveCurveScheduling
{
    public class TestWCFJobsVolatility
    {
        const double tolerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE;

        [OneTimeSetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            //return to default
            QaDao.RevertVolatilitiesToDefault("EEX_today");

            //update LastDateRunFor for Volatilities because Volatility job can be run only once for report date
            QaDao.UpdateLastDateRunForVolatilities("EEX_volatilities", DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd"));

          //  JobAPI.RunEutJobOncePerDay("EEX (power and gas)");
        }

        [Test]
        public void RunJob()
        {
            
            string today = DateTime.Today.ToString("yyyyMMdd");

            DateTime yesterdayDate = DateTime.Today.AddDays(-1);
            
            if (yesterdayDate.DayOfWeek == DayOfWeek.Saturday) yesterdayDate = yesterdayDate.AddDays(-1);
            if (yesterdayDate.DayOfWeek == DayOfWeek.Sunday) yesterdayDate = yesterdayDate.AddDays(-2);

            string yesterday = yesterdayDate.ToString("yyyyMMdd");

            //define report date for yesterday end of day price book
            DateTime dayBeforeYesterday = yesterdayDate.AddDays(-1);

            if (dayBeforeYesterday.DayOfWeek == DayOfWeek.Saturday) dayBeforeYesterday = dayBeforeYesterday.AddDays(-1);
            if (dayBeforeYesterday.DayOfWeek == DayOfWeek.Sunday) dayBeforeYesterday = dayBeforeYesterday.AddDays(-2);

            string dayBeforeYesterdayString = dayBeforeYesterday.ToString("yyyyMMdd");

            string[] reportdates = { today, yesterday, dayBeforeYesterdayString };
            //publishing pricebooks for today and yesterday
            RunPublishingPriceBookJob(reportdates);

            //run volatility job
            RunEEX_volatilitiesJobForDate(today);

            //geting pricecurve for yesterday - corresponding pricebook job for today
            //volatility should be updated
            PriceCurveDto yesterdayCurveDto = GetVolatilitiesArray(yesterdayDate);

            if (yesterdayCurveDto.PriceCurveVolatility != null)
                AssertCurrentVolatilityArray(yesterdayCurveDto);
            else Assert.Fail("Error getting PriceCurveVolatility for report date:" + yesterdayDate);

            //geting pricecurve for day before yesterday - corresponding pricebook job for yesterday
            //volatility should be same as in template
                PriceCurveDto curveDtoByTemplate = GetVolatilitiesArray(dayBeforeYesterday);
            if (curveDtoByTemplate.PriceCurveVolatility != null)
                AssertVolatilityArrayForYesterday(curveDtoByTemplate);
            else Assert.Fail("Error getting PriceCurveVolatility for report date: " + dayBeforeYesterday);

        }

        public void RunPublishingPriceBookJob(string[] reportdates )
        {
            foreach (string reportdate in reportdates)
            {
                //running job for publishing pricebook
                Dictionary<string, string> optionalParams = new Dictionary<string, string> {{"RunForDate", reportdate}};
                RunEEXJobs("EEX_today", "End Of Day Price Book Job", optionalParams);
            }
        }

        private void RunEEX_volatilitiesJobForDate(string reportdate)
        {
            //run EEX_volatilities job
            Dictionary<string, string> optionalParams = new Dictionary<string, string>();
            optionalParams.Add("RunForDate", reportdate);
            RunEEXJobs("EEX_volatilities", "Update volatilities and correlations", optionalParams);
            
        }

        private void RunEEXJobs(string jobName, string jobDescription, Dictionary<string, string> optionalParams)
        {
            try
            {
                int jobId = JobAPI.GetJobsIdByDescription(jobName, jobDescription);
                JobAPI.ExecuteAndAssertJob(jobId, optionalParams, 120);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }


        private PriceCurveDto GetVolatilitiesArray(DateTime reportdate)
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            PriceCurveDto testCurveDto = new PriceCurveDto();

            //Console.WriteLine(reportdate.ToString());

            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                FromDate = Convert.ToDateTime(reportdate.ToString("d")),
                ToDate = Convert.ToDateTime(reportdate.ToString("d")),
                LoadType = "Base",
                PriceBookAppendix = string.Empty,
                ReferenceAreaName = "DE/AT",
                ReportCurrencyIsoCode = "EUR",
                ReportDate = Convert.ToDateTime(reportdate.ToString("d")),
                Resolution = "Day",
                TemplateName = "EEX_today"
            };

            try
            {
                testCurveDto = service.GetPriceCurveByCriteria(criteria);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return testCurveDto;
        }

        private void AssertCurrentVolatilityArray(PriceCurveDto priceCurveDto)
        {
            Assert.AreNotEqual(0.1, priceCurveDto.PriceCurveVolatility.SummerLong,  "Value for PriceCurveVolatility.SummerLong should not match template value");
            Assert.AreNotEqual(0.17, priceCurveDto.PriceCurveVolatility.SummerMedium, "Value for PriceCurveVolatility.SummerMedium should not match template value");
            Assert.AreNotEqual(0.9, priceCurveDto.PriceCurveVolatility.SummerShort, "Value for PriceCurveVolatility.SummerShort should not match template value");

            Assert.AreNotEqual(0.1, priceCurveDto.PriceCurveVolatility.WinterLong, "Value for PriceCurveVolatility.WinterLong should not match template value");
            Assert.AreNotEqual(0.17, priceCurveDto.PriceCurveVolatility.WinterMedium, "Value for PriceCurveVolatility.WinterMedium should not match template value");
            Assert.AreNotEqual(0.9, priceCurveDto.PriceCurveVolatility.WinterShort, "Value for PriceCurveVolatility.WinterShort should not match template value");
        }

        private void AssertVolatilityArrayForYesterday(PriceCurveDto priceCurveDto)
        {
            Assert.AreEqual(0.1, priceCurveDto.PriceCurveVolatility.SummerLong, tolerance, "Wrong value for PriceCurveVolatility.SummerLong:");
            Assert.AreEqual(0.17, priceCurveDto.PriceCurveVolatility.SummerMedium, tolerance, "Wrong value for PriceCurveVolatility.SummerMedium:");
            Assert.AreEqual(0.5, priceCurveDto.PriceCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium, tolerance, "Wrong value for PriceCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium:");
            Assert.AreEqual(0.9, priceCurveDto.PriceCurveVolatility.SummerShort, tolerance, "Wrong value for PriceCurveVolatility.SummerShort:");

            Assert.AreEqual(0.1, priceCurveDto.PriceCurveVolatility.WinterLong, tolerance, "Wrong value for PriceCurveVolatility.WinterLong:");
            Assert.AreEqual(0.17, priceCurveDto.PriceCurveVolatility.WinterMedium, tolerance, "Wrong value for PriceCurveVolatility.WinterMedium:");
            Assert.AreEqual(0.5, priceCurveDto.PriceCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium, tolerance, "Wrong value for PriceCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium:");
            Assert.AreEqual(0.9, priceCurveDto.PriceCurveVolatility.WinterShort, tolerance, "Wrong value for PriceCurveVolatility.WinterShort:");
        }
    }
}
