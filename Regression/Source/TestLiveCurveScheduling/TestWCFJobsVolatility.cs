using System;
using System.Collections.Generic;
using System.Linq;
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
            DateTime yesterdayDate = DateTime.Today.AddDays(-1);
            if (yesterdayDate.DayOfWeek == DayOfWeek.Saturday) yesterdayDate = yesterdayDate.AddDays(-1);
            if (yesterdayDate.DayOfWeek == DayOfWeek.Sunday) yesterdayDate = yesterdayDate.AddDays(-2);

            //define report date for yesterday end of day price book
            DateTime dayBeforeYesterday = yesterdayDate.AddDays(-1);

            if (dayBeforeYesterday.DayOfWeek == DayOfWeek.Saturday) dayBeforeYesterday = dayBeforeYesterday.AddDays(-1);
            if (dayBeforeYesterday.DayOfWeek == DayOfWeek.Sunday) dayBeforeYesterday = dayBeforeYesterday.AddDays(-2);

            List<DateTime> reportDates = new List<DateTime> {yesterdayDate, DateTime.Today};
            //publishing pricebooks for yesterday and day before that
            RunEndOfDayPriceBookJob(reportDates);

            string yesterday = yesterdayDate.ToString("yyyyMMdd");
            //run volatility job
            RunEEX_volatilitiesJobForDate(yesterday);

            // Check that the last result for the vol job is success, if it was failure there's no point limping on here!
            if (QaDao.GetLastResultCodeForVolatilities("EEX_volatilities") != 0)
                Assert.Fail($"Running Volatilities and Correlations job EEX_volatilities for '{yesterdayDate}' failed.");

            //getting pricecurve for yesterday - volatilities should be updated
            PriceCurveVolatilityDto yesterdaysVol = GetVolatilityParameters(yesterdayDate);

            if (yesterdaysVol != null)
                AssertVolatilityParametersAreNotEqualToInitialParameters(yesterdaysVol);
            else Assert.Fail("Error getting PriceCurveVolatility for report date:" + yesterdayDate);

            //getting pricecurve for day before yesterday - volatilities should be same as in template
            PriceCurveVolatilityDto dayBeforeYesterdaysVol = GetVolatilityParameters(dayBeforeYesterday);
            if (dayBeforeYesterdaysVol != null)
                AssertVolatilityParametersAreEqualToInitialParameters(dayBeforeYesterdaysVol);
            else Assert.Fail("Error getting PriceCurveVolatility for report date: " + dayBeforeYesterday);
        }

        public void RunEndOfDayPriceBookJob(IList<DateTime> reportDates)
        {
            foreach (DateTime reportDate in reportDates)
            {
                //running job for publishing pricebook
                Dictionary<string, string> optionalParams = new Dictionary<string, string> {{"RunForDate", reportDate.ToString("yyyyMMdd")}};
                RunJobs("EEX_today", "End Of Day Price Book Job", optionalParams);
            }
        }

        private void RunEEX_volatilitiesJobForDate(string reportdate)
        {
            //run EEX_volatilities job
            Dictionary<string, string> optionalParams = new Dictionary<string, string> {{"RunForDate", reportdate}};
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
        //Run Volatilities ans correlations job for (period -2 days)
        //Compare that first available PriceCurveVolatility is not equal to the last available
        //Precondition - prices for area should be available for creating EEX_today price books
        //**********************************
        // Period should be set to relevant
        //*********************************
        public void CreatePriceBooksForPeriodAndComparePriceCurveVolatilities()
        {
            // *** setting period ***
            DateTime inputStartDate = new DateTime(2018, 01, 04);
            DateTime inputEndDate = new DateTime(2018, 01, 31);

            List<DateTime> priceBookJobDates = new List<DateTime>();
            List<DateTime> volatilitiesDates = new List<DateTime>();
            DateTime inputDate = inputStartDate;
            do
            {
                // Skip first day, Sundays and Mondays (price book job has "End of Yesterday" mode)
                if (!inputDate.Equals(inputStartDate) && inputDate.DayOfWeek != DayOfWeek.Sunday && inputDate.DayOfWeek != DayOfWeek.Monday)
                    priceBookJobDates.Add(inputDate);

                // Skip first day, last day, Sundays and Saturdays
                if (!inputDate.Equals(inputStartDate) && !inputDate.Equals(inputEndDate) && inputDate.DayOfWeek != DayOfWeek.Sunday && inputDate.DayOfWeek != DayOfWeek.Saturday)
                    volatilitiesDates.Add(inputDate);

                inputDate = inputDate.AddDays(1);

            } while (inputDate <= inputEndDate);

            RunEndOfDayPriceBookJob(priceBookJobDates);

            //Run "Update volatilities and correlations" job
            foreach (DateTime vol in volatilitiesDates)
            {
                string reportDay = vol.ToString("yyyyMMdd");
                RunEEX_volatilitiesJobForDate(reportDay);
            }

            //Get 3 pt vol curve parameters for first day
            PriceCurveVolatilityDto firstDayVols = GetVolatilityParameters(volatilitiesDates[0]);

            //Get 3 pt vol curve parameters for last day
            PriceCurveVolatilityDto lastDayVols = GetVolatilityParameters(inputEndDate.AddDays(-1));

            //Check that volatility for first date is not the same as for last date
            AssertVolatilityParametersAreNotEqual(firstDayVols, lastDayVols);

            //Check that numbers for PriceCurveVolatility are as expected for the last date (30-Jan-2018) => no changes in algorithm
            const double expectedShort = 2.22674856275395;
            const double expectedMedium = 0.14495680491414009;
            const double expectedLong = 0.10002363675755585;
            AssertVolatilityParameters(lastDayVols, expectedShort, expectedMedium, expectedLong, inputEndDate.AddDays(-1));
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

            Dictionary<DateTime, PriceCurveVolatilityDto> allPriceCurveVolatilities = new Dictionary<DateTime, PriceCurveVolatilityDto>();
            
            //Run "Update volatilities and correlations" job
            foreach (DateTime date in volatilitiesDates)
            {
                string reportDay = date.ToString("yyyyMMdd");
                RunEEX_volatilitiesJobForDate(reportDay); //***
              
                if (date != volatilitiesDates.Last())
                {
                    PriceCurveVolatilityDto vol = GetVolatilityParameters(date);
                    if (vol != null) allPriceCurveVolatilities.Add(date, vol);
                }
            }

            foreach (KeyValuePair<DateTime, PriceCurveVolatilityDto> kvp in allPriceCurveVolatilities)
            {
                Console.WriteLine("{0}; SummerLong = {1}; SummerMedium = {2}; SummerShort = {3}; WinterLong = {4}; WinterMedium = {5}; WinterShort = {6}", kvp.Key, 
                    kvp.Value.SummerLong, kvp.Value.SummerMedium, kvp.Value.SummerShort,
                    kvp.Value.WinterLong, kvp.Value.WinterMedium, kvp.Value.WinterShort);
            }

        }

        private static PriceCurveVolatilityDto GetVolatilityParameters(DateTime reportdate)
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

            return testCurveDto.PriceCurveVolatility;
        }

        private static void AssertVolatilityParametersAreNotEqualToInitialParameters(PriceCurveVolatilityDto volatilityParameters)
        {
            Assert.AreNotEqual(0.1, volatilityParameters.SummerLong,  "Value for SummerLong should not match template value");
            Assert.AreNotEqual(0.17, volatilityParameters.SummerMedium, "Value for SummerMedium should not match template value");
            Assert.AreNotEqual(0.9, volatilityParameters.SummerShort, "Value for SummerShort should not match template value");

            Assert.AreNotEqual(0.1, volatilityParameters.WinterLong, "Value for WinterLong should not match template value");
            Assert.AreNotEqual(0.17, volatilityParameters.WinterMedium, "Value for WinterMedium should not match template value");
            Assert.AreNotEqual(0.9, volatilityParameters.WinterShort, "Value for WinterShort should not match template value");
        }

        private static void AssertVolatilityParametersAreEqualToInitialParameters(PriceCurveVolatilityDto volatilityParameters)
        {
            Assert.AreEqual(0.1, volatilityParameters.SummerLong, tolerance, "Wrong value for SummerLong:");
            Assert.AreEqual(0.17, volatilityParameters.SummerMedium, tolerance, "Wrong value for SummerMedium:");
            Assert.AreEqual(0.9, volatilityParameters.SummerShort, tolerance, "Wrong value for SummerShort:");

            Assert.AreEqual(0.1, volatilityParameters.WinterLong, tolerance, "Wrong value for WinterLong:");
            Assert.AreEqual(0.17, volatilityParameters.WinterMedium, tolerance, "Wrong value for WinterMedium:");
            Assert.AreEqual(0.9, volatilityParameters.WinterShort, tolerance, "Wrong value for WinterShort:");
        }

        private static void AssertVolatilityParametersAreNotEqual(PriceCurveVolatilityDto first, PriceCurveVolatilityDto second)
        {
            Assert.AreNotEqual(first.SummerLong, second.SummerLong, "Expected different values for SummerLong");
            Assert.AreNotEqual(first.SummerMedium, second.SummerMedium, "Expected different values for SummerMedium");
            Assert.AreNotEqual(first.SummerShort, second.SummerShort, "Expected different values for SummerShort");

            Assert.AreNotEqual(first.WinterLong, second.WinterLong, "Expected different values for WinterLong");
            Assert.AreNotEqual(first.WinterMedium, second.WinterMedium, "Expected different values for WinterMedium");
            Assert.AreNotEqual(first.WinterShort, second.WinterShort, "Expected different values for WinterShort");
        }

        private static void AssertVolatilityParameters(PriceCurveVolatilityDto volatilityParameters, double expectedShort, double expectedMedium, double expectedLong, DateTime date)
        {
            string errorMessage = $"value in PriceCurveVolatility has changed for {date}.";
           
            Assert.AreEqual(0, expectedLong / volatilityParameters.SummerLong - 1, tolerance, "SummerLong " + errorMessage);
            Assert.AreEqual(0, expectedMedium / volatilityParameters.SummerMedium - 1, tolerance, "SummerMedium " + errorMessage);
            Assert.AreEqual(0, expectedShort / volatilityParameters.SummerShort - 1, tolerance, "SummerShort " + errorMessage);

            Assert.AreEqual(0, expectedLong / volatilityParameters.WinterLong-1, tolerance, "WinterLong " + errorMessage);
            Assert.AreEqual(0, expectedMedium / volatilityParameters.WinterMedium-1, tolerance, "WinterMedium " + errorMessage);
            Assert.AreEqual(0, expectedShort / volatilityParameters.WinterShort-1, tolerance, "WinterShort " + errorMessage);
        }
    }
}

