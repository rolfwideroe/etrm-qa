using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
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
        public void OneTimeSetup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
          
            //we need up2date prices to publish price books 
            JobAPI.RunEutJobOncePerDayWithoutAssert("EEX (power and gas)");

        }

        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            //return to default
            QaDao.RevertVolatilitiesToDefault("EEX_today");

            //update LastDateRunFor for Volatilities because Volatility job can be run only once for report date
            QaDao.UpdateLastDateRunForVolatilities("EEX_volatilities","2017-12-31");
            
        }

        
        [Test]
        public void CreatePriceBookAndVolatilityJobYesterday()
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

            string[] reportdates = {today, yesterday, dayBeforeYesterdayString};
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

        public void RunPublishingPriceBookJob(string[] reportdates)
        {
            foreach (string reportdate in reportdates)
            {
                //running job for publishing pricebook
                Dictionary<string, string> optionalParams = new Dictionary<string, string> {{"RunForDate", reportdate}};
                RunJobs("EEX_today", "End Of Day Price Book Job", optionalParams);
            }
        }

        private void RunEEX_volatilitiesJobForDate(string reportdate)
        {
            //run EEX_volatilities job
            Dictionary<string, string> optionalParams = new Dictionary<string, string>();
            optionalParams.Add("RunForDate", reportdate);
            RunJobs("EEX_volatilities", "Update volatilities and correlations", optionalParams);

        }

        private void RunJobs(string jobName, string jobDescription, Dictionary<string, string> optionalParams)
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


        [Test]
        [Category("Debug")]
        //This test will generate End of Day Price Books for given period
        //Run Volatilities ans correlations job for (preiod -2 days)
        //Compare that first available PriceCurveVolatility is not equal to the last available
        //Precondition - prices for area should be available for creating EEX_today price books
        //**********************************
        // Period should be set to relevant
        //*********************************
        public void CreatePriceBooksForPeriodAndComparePriceCurveVolatilities()
        {
            // *** setting period ***
            DateTime inputStartDate = new DateTime(2018, 01, 04);
            DateTime inputEndDate = new DateTime(2018, 01, 30);

            var dates = Enumerable.Range(0, (inputEndDate - inputStartDate).Days + 1)
                .Select(day => inputStartDate.AddDays(day)).ToList();

            List<DateTime> volatilitiesDates = new List<DateTime>();

            //removing Sundays and Saturday (volatility job has "End of Yesterday" mode)
            foreach (var day in dates)
            {
                if ( day.DayOfWeek != DayOfWeek.Sunday && day.DayOfWeek != DayOfWeek.Saturday)
                    volatilitiesDates.Add(day);
            }
            
            //Generate price books
            foreach (var day in dates)
            {
                string reportDay = day.ToString("yyyyMMdd");
                Dictionary<string, string> optionalParams = new Dictionary<string, string> { { "RunForDate", reportDay } };
               RunJobs("EEX_today", "End Of Day Price Book Job", optionalParams);
            }

            //removing first 2 days - because volatility job has "End of Yesterday" mode and it will not have results for first 2 days
            volatilitiesDates.RemoveAt(0);
            volatilitiesDates.RemoveAt(1);

            //Run "Update volatilities and correlations" job
            foreach (var vol in volatilitiesDates)
            {
                string reportDay = vol.ToString("yyyyMMdd");
                RunEEX_volatilitiesJobForDate(reportDay);
            }
            
            //Get Volatility Array for first available day
            PriceCurveDto priceCurveDtoVolatilityFirst = GetVolatilitiesArray(volatilitiesDates.FirstOrDefault());

            //Get Volatility Array for the available last day
            DateTime last = volatilitiesDates.Last().AddDays(-1); //get curve for "yesterday" because volatility job has "End of Yesterday" mode
            PriceCurveDto priceCurveDtoVolatilitySecond = GetVolatilitiesArray(last);

            //Check that volatility for first date is not the same as for last date
            AssertTwoVolatilityArrays(priceCurveDtoVolatilityFirst, priceCurveDtoVolatilitySecond);

        }
       
       // not for regression        
       // [Test]
        [Category("Debug")]
        //*********************************
        //This test will generate End of Day Price Books for given period
        //Run Volatilities ans correlations job for (preiod -2 days)
        //After it will compare that today PriceCurveVolatility is different form yesterday for period
        //It will write results to console: PriceCurveDtoVolatility for report date 
        //Results can be copied to Excel for further analyse
        //**********************************
        // Period should be set to relevant
        //*********************************
        public void CreatePriceBooksForPeriodAndPrintPriceCurveVolatilitiesFromDayToDay()
        {
            // *** setting period ***
            DateTime inputStartDate = new DateTime(2018, 01, 03);
            DateTime inputEndDate = new DateTime(2018, 01, 30);

            var dates = Enumerable.Range(0, (inputEndDate - inputStartDate).Days + 1)
                .Select(day => inputStartDate.AddDays(day)).ToList();

            List<DateTime> volatilitiesDates = new List<DateTime>();

            //removing Sundays and Saturday (volatility job has "End of Yesterday" mode)
            foreach (var day in dates)
            {
                if (day.DayOfWeek != DayOfWeek.Sunday && day.DayOfWeek != DayOfWeek.Saturday)
                    volatilitiesDates.Add(day);
            }

            //Generate price books
            foreach (var day in dates)
            {
                string reportDay = day.ToString("yyyyMMdd");
                Dictionary<string, string> optionalParams = new Dictionary<string, string> { { "RunForDate", reportDay } };
                RunJobs("EEX_today", "End Of Day Price Book Job", optionalParams); //***
            }

            //removing first 2 days - because volatility job has "End of Yesterday" mode and it will not have results for first 2 days
            volatilitiesDates.RemoveAt(0);
            volatilitiesDates.RemoveAt(1);

            Dictionary<DateTime, PriceCurveDto> allPriceCurveDtoVolatility = new Dictionary<DateTime, PriceCurveDto>();
            
            //Run "Update volatilities and correlations" job
            foreach (var vol in volatilitiesDates)
            {
                string reportDay = vol.ToString("yyyyMMdd");
                RunEEX_volatilitiesJobForDate(reportDay); //***
              
                if (vol != volatilitiesDates.Last())
                {
                    PriceCurveDto priceCurveDtoVolatility = GetVolatilitiesArray(vol);
                    if (priceCurveDtoVolatility.PriceCurveVolatility != null)
                        allPriceCurveDtoVolatility.Add(vol, priceCurveDtoVolatility);
                }
            }

            foreach (KeyValuePair<DateTime, PriceCurveDto> kvp in allPriceCurveDtoVolatility)
            {
                Console.WriteLine("{0}; SummerLong = {1}; SummerMedium = {2}; SummerShort = {3}; WinterLong = {4}; WinterMedium = {5}; WinterShort = {6}", kvp.Key, 
                    kvp.Value.PriceCurveVolatility.SummerLong, kvp.Value.PriceCurveVolatility.SummerMedium, kvp.Value.PriceCurveVolatility.SummerShort,
                    kvp.Value.PriceCurveVolatility.WinterLong, kvp.Value.PriceCurveVolatility.WinterMedium, kvp.Value.PriceCurveVolatility.WinterShort);
            }

        }

      
        private PriceCurveDto GetVolatilitiesArray(DateTime reportdate)
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            PriceCurveDto testCurveDto = new PriceCurveDto();

            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                FromDate = Convert.ToDateTime(reportdate.ToString("d")),
                ToDate = Convert.ToDateTime(reportdate.ToString("d")),
                LoadType = "Base",
                PriceBookAppendix = string.Empty,
                ReportCurrencyIsoCode = "EUR",
                ReportDate = Convert.ToDateTime(reportdate.ToString("d")),
                Resolution = "Day",
                ReferenceAreaName = "DE/AT",
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


        private void AssertTwoVolatilityArrays(PriceCurveDto priceCurveDtoVolatilityFirst, PriceCurveDto priceCurveDtoVolatilitySecond)
        {
            Assert.IsNotNull(priceCurveDtoVolatilityFirst.PriceCurveVolatility, "PriceCurveVolatility for first date is null");
            Assert.IsNotNull(priceCurveDtoVolatilitySecond.PriceCurveVolatility, "PriceCurveVolatility for last date is null");
            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.SummerLong, priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerLong, "Expected different values for SummerLong");
            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.SummerMedium, priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerMedium, "Expected different values for SummerMedium");
            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.SummerShort, priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerShort, "Expected different values for SummerShort");

            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.WinterLong, priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterLong, "Expected different values for WinterLong");
            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.WinterMedium, priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterMedium, "Expected different values for WinterMedium");
            Assert.AreNotEqual(priceCurveDtoVolatilityFirst.PriceCurveVolatility.WinterShort, priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterShort, "Expected different values for WinterShort");


            //Check that numbers for PriceCurveVolatility are same for DateTime(2018, 01, 30) => no changes in algorithm

            string errorMessage = "value in PriceCurveVolatility have been changed for DateTime(2018, 01, 30).";
           
           
            double expectedLong = 0.0926842384910094;
            double expectedMedium = 0.139964886263095;
            double expectedShort = 2.21220688377257;

            //Console.WriteLine(priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerShort + " ; " + priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerMedium + " ; "+ priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerLong);

            Assert.AreEqual(0, ((expectedLong / priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerLong) - 1), tolerance, "SummerLong " + errorMessage);
            Assert.AreEqual(0, ((expectedMedium / priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerMedium) - 1), tolerance, "SummerMedium " + errorMessage);
            Assert.AreEqual(0, ((expectedShort / priceCurveDtoVolatilitySecond.PriceCurveVolatility.SummerShort) - 1), tolerance, "SummerShort " + errorMessage);

            Assert.AreEqual(0, (expectedLong / priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterLong-1), tolerance, "WinterLong " + errorMessage);
            Assert.AreEqual(0, (expectedMedium / priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterMedium-1), tolerance, "WinterMedium " + errorMessage);
            Assert.AreEqual(0, (expectedShort / priceCurveDtoVolatilitySecond.PriceCurveVolatility.WinterShort-1), tolerance, "WinterShort " + errorMessage);

        }


    }
}

