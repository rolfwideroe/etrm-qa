using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;


namespace TestWCFCurveService
{
    [TestFixture]
    public class TestWCFCurveServiceGetPriceCurve
    {
        private const string PriceCurveFilePath = "Testfiles\\PriceCurve\\";

        private static readonly IEnumerable<string> PriceCurve = TestCasesFileEnumeratorByFolder.TestCaseFiles(PriceCurveFilePath);

        private PriceCurveCriteria GetDefaultPriceCurveCriteria()
        {
            PriceCurveCriteria criteria = new PriceCurveCriteria
            {
                FromDate = new DateTime(2011, 01, 01),
                ToDate = new DateTime(2011, 12, 31),
                LoadType = "Base",
                PriceBookAppendix = "",
                ReferenceAreaName = "NPS",
                ReportCurrencyIsoCode = "EUR",
                ReportDate = new DateTime(2011,11,01),
                Resolution = "Hour",
                TemplateName = "NPXSYSALL"
            };

            return criteria;
        }



        [Test] // DateTimeKind should be only Unspecified 
        public void FailCurveServiceDateTimeKindisNotUnspecified()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();
            DateTime reportDate = new DateTime(2011, 11, 11, 0, 0, 0, DateTimeKind.Utc);
            DateTime fromDateTime = new DateTime(2011, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            DateTime toDateTime = new DateTime(2011, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            PriceCurveCriteria criteria = GetDefaultPriceCurveCriteria();
            criteria.ReportDate = reportDate;

            string reportDateExceptionMessage ="Failed to return a curve: Variable 'ReportDate' must be unspecified kind";

            Assert.Throws(Is.TypeOf<FaultException<ExceptionDetail>>().And.Message.EqualTo(reportDateExceptionMessage),delegate () { service.GetPriceCurveByCriteria(criteria); },"ReportDate failed");

           // criteria = GetDefaultPriceCurveCriteria();

           // criteria.FromDate = fromDateTime;

           // Assert.Throws(Is.TypeOf<FaultException<ExceptionDetail>>().And.Message.EqualTo(reportDateExceptionMessage),delegate () { service.GetPriceCurveByCriteria(criteria); },"FromDate failed");

           // Assert.Throws(Is.TypeOf<FaultException<ExceptionDetail>>().And.Message.EqualTo(reportDateExceptionMessage),delegate () { service.GetPriceCurveByCriteria(criteria); },"ReportDate failed");


        }

        [Test]
        public void TestCurveValueDateKinds()
        {
            ICurveService service = WCFClientUtil.GetCurveServiceServiceProxy();

            PriceCurveCriteria criteria = GetDefaultPriceCurveCriteria();
            
            PriceCurveDto dto= service.GetPriceCurveByCriteria(criteria);

            DateTime curveDateTime = dto.ValueDates[0];
            DateTime curveDateTimeUtc = dto.ValueDatesUtc[0];
            DateTime reportDate = dto.ReportDate;

            Assert.AreEqual(DateTimeKind.Unspecified,curveDateTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc,curveDateTimeUtc.Kind);
            Assert.AreEqual(DateTimeKind.Unspecified,reportDate.Kind);

        }


        [Test, Timeout(1000 * 1000),TestCaseSource("PriceCurve")]
        public void TestPriceCurve(string testFile)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), PriceCurveFilePath + testFile);
          
            TestPriceCurveDto(filepath);
        }

        private const string IndexCurveFilePath = "Testfiles\\IndexCurve\\";

        private static readonly IEnumerable<string> IndexCurve = TestCasesFileEnumeratorByFolder.TestCaseFiles(IndexCurveFilePath);

        [Test, Timeout(1000 * 1000), TestCaseSource("IndexCurve")]
        public void TestIndexCurve(string testFile)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), IndexCurveFilePath + testFile);

            TestPriceCurveDto(filepath);
        }

        private void TestPriceCurveDto(string testFilePath)
        {


            CurveTestCase curveTestCase = TestXmlTool.Deserialize<CurveTestCase>(testFilePath);
            ElvizConfiguration[] elvizConfigurations = curveTestCase.InputData.ElvizConfigurations;

            if (!string.IsNullOrEmpty(curveTestCase.ExpectedValues.ErrorMessage))
            {
                try
                {
                    CurveTestUtil.GetCurve(curveTestCase);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(curveTestCase.ExpectedValues.ErrorMessage, ex.Message,
                        "Test Expected to Fail but other Exception was caught: " + ex.Message);

                }
            }
            else
            {
                try
                {

                    QaPriceCurveDtoWrapper wrapper = CurveTestUtil.GetCurve(curveTestCase);

                    //Addititonal information, get number values from percent value
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.WinterShort,
                        wrapper.PriceCurveVolatility.WinterShort, "Volatilty : WinterShort not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.WinterMedium,
                        wrapper.PriceCurveVolatility.WinterMedium, "Volatilty : WinterMedium not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.WinterLong,
                        wrapper.PriceCurveVolatility.WinterLong, "Volatilty : WinterLong not equal");
                    Assert.AreEqual(
                        curveTestCase.ExpectedValues.ExpectedCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium,
                        wrapper.PriceCurveVolatility.WinterNumberOfYearsBetweenShortAndMedium,
                        "Volatilty : NumberOfYears not equal");

                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.SummerShort,
                        wrapper.PriceCurveVolatility.SummerShort, "Volatilty : SummerShort not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.SummerMedium,
                        wrapper.PriceCurveVolatility.SummerMedium, "Volatilty : SummerMedium not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedCurveVolatility.SummerLong,
                        wrapper.PriceCurveVolatility.SummerLong, "Volatilty : SummerLong not equal");
                    Assert.AreEqual(
                        curveTestCase.ExpectedValues.ExpectedCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium,
                        wrapper.PriceCurveVolatility.SummerNumberOfYearsBetweenShortAndMedium,
                        "Volatilty : NumberOfYears not equal");


                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedProperties.CurveType, wrapper.CurveType,
                        "CurveType not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedProperties.VolumeUnit, wrapper.VolumeUnit,
                        "VolumeUnit not equal");
                    Assert.AreEqual(curveTestCase.ExpectedValues.ExpectedProperties.TimeZone, wrapper.TimeZone,
                        "TimeZone not equal");

                    List<DateTimeValue> dateTimevalues = wrapper.GetCurveValues();

                    List<DateTimeValue> expectedTimevalues =
                        curveTestCase.ExpectedValues.ExpectedCurveValues.Select(d => new DateTimeValue(d.Date, d.Value))
                            .ToList();

                    foreach (DateTimeValue record in wrapper.GetCurveValues())
                    {
                        string s = "";
                        s = " <ExpectedCurveValue Date=\"" + record.DateTime.ToString("yyyy-MM-ddTHH:mm:ss") +
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
                            double relativeError = (actual/expected) - 1;
                            string errorMessage = "Failed for record : " + dateTimevalues[i].DateTime +
                                                  " : Expected value is : " +
                                                  expected + " , but Actual value was " + actual;
                            //Assert value
                            Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,
                                errorMessage);
                        }
                    }

                    //    if(curveTest.ResultInformation.ResultValue.ToUpper()=="EXCEPTION") Assert.Fail("Test Passed, but expected to fail with error :\n "+curveTest.ResultInformation.ErrorMessage);

                    Assert.AreEqual(curveTestCase.InputData.LoadType, wrapper.ElvizDto.LoadType, "LoadType not equal");
                    Assert.AreEqual(curveTestCase.InputData.PriceBookAppendix, wrapper.ElvizDto.PriceBookAppendix,
                        "PriceBookAppendix not equal");

                    if (!wrapper.ElvizDto.IsIndexPriceCurve)
                        Assert.AreEqual(curveTestCase.InputData.ReferenceAreaName, wrapper.ElvizDto.ReferenceAreaName,
                            "ReferenceArea not equal");

                    Assert.AreEqual(curveTestCase.InputData.ReportCurrencyIsoCode, wrapper.ElvizDto.Currency,
                        "Currency not equal");
                    Assert.AreEqual(curveTestCase.InputData.ReportDate, wrapper.ElvizDto.ReportDate, "ReportDate not equal");
                    Assert.AreEqual(curveTestCase.InputData.Resolution, wrapper.ElvizDto.PriceSeriesResolution,
                        "Resolution not equal");
                    Assert.AreEqual(curveTestCase.InputData.TemplateName, wrapper.ElvizDto.PriceBookName,
                        "TemplateName not equal");

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
        }
    }

  
  

}
