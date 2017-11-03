using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ElvizTestUtils;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;
using TestFileWatcherWithUtils;

namespace CurveImport
{
    [TestFixture]
    public class CurveImportFileWatcher
    {
        private static readonly IEnumerable<string> TestFilesCurveImport =
            TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFilesCurveImport");
        private const string PriceCurveExpectedValues = "TestFilesCurveImport\\ExpectedValues\\";
      

        [Test, Timeout(1000*100), TestCaseSource("TestFilesCurveImport")]
        public void CurveImportTest(string fileName)
        {
            //delete previous resalts from folder
            //copy test files
            CurveImportUtil.PrepareTestFolder(fileName);

            CurveImportUtil.WaitWhileFileIsBeingProcessed(fileName);

            CurveImportUtil.getTestResult(fileName);

            //for negativ test just check that import failed
            if (CurveImportUtil.shouldTestCaseFail(fileName))
            {
                if (!CurveImportUtil.didTestFail(fileName))
                    throw new ApplicationException(string.Format("Test {0} should FAIL but PASSED", fileName));
                return;
            }

            //for positive tests import curve... ->
            if (CurveImportUtil.shouldTestCasePass(fileName) && !CurveImportUtil.didTestSucceed(fileName))
                throw new ApplicationException(string.Format("Test {0} should PASS but FAILED with message {1}",
                    fileName, CurveImportUtil.getFailedMessageText(fileName)));
            // ->...and get back price curve 
            String expectedValuesPath = Path.Combine(Directory.GetCurrentDirectory(), PriceCurveExpectedValues);
            DirectoryInfo expectedValuesDirectory = new DirectoryInfo(expectedValuesPath);

            string priceBookName = "";
            priceBookName = fileName.Split('-').FirstOrDefault();

                IEnumerable < FileInfo > filesToTest = expectedValuesDirectory.GetFiles().Where(x => x.Name.StartsWith(priceBookName));

            foreach (FileInfo expectedValuesFileName in filesToTest)
            {
               Console.WriteLine(expectedValuesFileName.Name);
                TestPriceCurveDto(Path.Combine(Directory.GetCurrentDirectory(), PriceCurveExpectedValues,
                    expectedValuesFileName.Name));
            }

        }

        //Calling GetPriceCurveByCriteria and compare prices set with expected values
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
                        PriceBookAppendix = null,//curveTest.InputData.PriceBookAppendix,
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
                                "The actual Date : " + dateTimevalues[i].DateTime +
                                " did not match the expected date : " +
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
                                Assert.AreEqual(0, relativeError,
                                    GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,
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
       
    }
}
    

