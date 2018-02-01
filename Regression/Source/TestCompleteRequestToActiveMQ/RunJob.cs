using System;
using System.Collections.Generic;
using System.Linq;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteRequestToActiveMQ
{
    class RunJob
    {
        public static void RunJobByDescription(string description, string jobtype)
        {
            //running job for publishing pricebook
            int curveJobId = JobAPI.GetJobsIdByDescription(description, jobtype);
            JobAPI.ExecuteAndAssertJob(curveJobId, 600);
        }

        public static void RunJobById(int jobId)
        {
            try
            {
                JobAPI.ExecuteAndAssertJob(jobId, 600);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Test]
        public static string RunEUTJob(string description, string jobType)
        {
           // string description = "GME";
            string resultmessage = "Job execution failed: " + description;

            try
            {
                JobAPI.RunEutJobOncePerDay(description);
            }
            catch (Exception)
            {
                return resultmessage;
            }

            return "Success";
        }

    }

    class CurveAssert
    {
        [Test]
        public static  IList<string> AssertPriceCurveDto(string testFilePath)
        {
            const double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE;
            IList<string> errors = new List<string>();

            //string testFilePath = @"C:\Git\qa-etrm\Regression\Source\TestComplete\CurveServer\EditRepublishPriceBooks\TestFiles\ExpectedCurves\ExpectedCurve_NPX_HourPrices_20171108_Profiled.xml";

            CurveTestCase curveTest = TestXmlTool.Deserialize<CurveTestCase>(testFilePath);

            DateTime reportDate = curveTest.InputData.ReportDate;
            string dateformat = "yyyy-MM-dd";
            string time = DateTime.Now.ToString(dateformat);

            if (reportDate == DateTime.MinValue) reportDate = DateTime.Parse(time);

            //if ElvizConfigurations should be updated  - add try-finelly with set-return to default
            // ElvizConfiguration[] elvizConfigurations = curveTest.InputData.ElvizConfigurations;
            //if (elvizConfigurations != null)
            //{
            //    ElvizConfigurationTool utility = new ElvizConfigurationTool();

            //    utility.UpdateConfiguration(elvizConfigurations);
            //}

            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                CurrencyQuote = curveTest.InputData.CurrencyQuote,
                FromDate = curveTest.InputData.FromDate,
                ToDate = curveTest.InputData.ToDate,
                LoadType = curveTest.InputData.LoadType,
                PriceBookAppendix = null, //curveTest.InputData.PriceBookAppendix,
                ReferenceAreaName = curveTest.InputData.ReferenceAreaName,
                ReportCurrencyIsoCode = curveTest.InputData.ReportCurrencyIsoCode,
                ReportDate = reportDate,
                Resolution = curveTest.InputData.Resolution,
                TemplateName = curveTest.InputData.TemplateName,
                UseLiveCurrencyRates = curveTest.InputData.UseLiveCurrencyRates,
				CurvePriceType = curveTest.InputData.CurvePriceType
            };
            

            try
            {
               PriceCurveDto testCurveDto = service.GetPriceCurveByCriteria(criteria);
               QaPriceCurveDtoWrapper wrapper = new QaPriceCurveDtoWrapper(service.GetPriceCurveByCriteria(criteria));
               

               if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterShort - wrapper.PriceCurveVolatility.WinterShort) > tollerance)
                    errors.Add("Volatilty : WinterShort not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterMedium - wrapper.PriceCurveVolatility.WinterMedium) > tollerance)
                    errors.Add("Volatilty : WinterMedium not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterLong - wrapper.PriceCurveVolatility.WinterLong) > tollerance)
                    errors.Add("Volatilty : WinterLong not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium - wrapper.PriceCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium) > tollerance)
                    errors.Add("Volatilty : NumberOfYears not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerShort - wrapper.PriceCurveVolatility.SummerShort) > tollerance)
                    errors.Add("Volatilty : SummerShort not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerMedium - wrapper.PriceCurveVolatility.SummerMedium) > tollerance)
                     errors.Add("Volatilty : SummerMedium not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerLong - wrapper.PriceCurveVolatility.SummerLong) > tollerance)
                    errors.Add("Volatilty : SummerLong not equal");

                if (Math.Abs(curveTest.ExpectedValues.ExpectedCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium- wrapper.PriceCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium) > tollerance)
                    errors.Add("Volatilty : NumberOfYears not equal");


                if(curveTest.ExpectedValues.ExpectedProperties.CurveType != wrapper.CurveType)
                    errors.Add("CurveType not equal");
                if(curveTest.ExpectedValues.ExpectedProperties.VolumeUnit != wrapper.VolumeUnit)
                    errors.Add("VolumeUnit not equal");
                if(curveTest.ExpectedValues.ExpectedProperties.TimeZone != wrapper.TimeZone)
                    errors.Add("TimeZone not equal");

                List<DateTimeValue> dateTimevalues = wrapper.GetCurveValues();

                List<DateTimeValue> expectedTimevalues =
                    curveTest.ExpectedValues.ExpectedCurveValues.Select(d => new DateTimeValue(d.Date, d.Value))
                        .ToList();
                //for creating expeced values
                foreach (DateTimeValue record in wrapper.GetCurveValues())
                {
                    string s = "";
                    // s += record.DateTime + " ; " + record.Value;
                    s += "<ExpectedCurveValue Date=\"" + record.DateTime.ToString("yyyy-MM-ddTHH:mm:ss") +
                         "\" Value=\"" + record.Value + "\"/>";
                      Console.WriteLine(s);
                   // Console.WriteLine(record.Value);
                }

                if(expectedTimevalues.Count != dateTimevalues.Count)
                {
                    errors.Add("Test case contains wrong number of expected values");
                    return errors;
                }


                for (int i = 0; i < expectedTimevalues.Count; i++)
                {
                    //Assert Data
                    if(expectedTimevalues[i].DateTime != dateTimevalues[i].DateTime)
                        errors.Add("The actual Date : " + dateTimevalues[i].DateTime + " did not match the expected date : " + expectedTimevalues[i].DateTime + " at index = " + i);

                    double actual = dateTimevalues[i].Value;
                    double expected = expectedTimevalues[i].Value;
                    string errorMessage = "Failed for record : " + dateTimevalues[i].DateTime +
                                              " : Expected value is : " +
                                              expected + " , but Actual value was " + actual;


                    if(Math.Abs(actual - expected) > tollerance) errors.Add(errorMessage);
                    
                }

                if(curveTest.InputData.LoadType != testCurveDto.LoadType)
                    errors.Add("LoadType not equal");

                //Assert.AreEqual(curveTest.InputData.PriceBookAppendix, testCurveDto.PriceBookAppendix,
                //    "PriceBookAppendix not equal");

                if(!testCurveDto.IsIndexPriceCurve)
                   if(curveTest.InputData.ReferenceAreaName != testCurveDto.ReferenceAreaName)
                      errors.Add("ReferenceArea not equal");

                if(curveTest.InputData.ReportCurrencyIsoCode != testCurveDto.Currency)
                    errors.Add("Currency not equal");
                if(reportDate != testCurveDto.ReportDate)
                    errors.Add("ReportDate not equal");
                if(curveTest.InputData.Resolution != testCurveDto.PriceSeriesResolution)
                    errors.Add("Resolution not equal");
                if(curveTest.InputData.TemplateName != testCurveDto.PriceBookName)
                   errors.Add("TemplateName not equal");
            }

            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
                errors.Add(ex.Message);
            }
            //print out alll errors
            //foreach (var item in errors)
            //{
            //    Console.WriteLine(item);
            //}

            return errors;
        }
    }

}
