using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.VolatilityServiceReference;
using NUnit.Framework;

namespace TestWCFVolatilityService
{
    [TestFixture]
    public class TestVolatilityService
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        private Option GetDefaultOption()
        {
            return new Option
            {
                OptionExpiry = new DateTime(2012, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified),
                OptionFromDate = new DateTime(2012, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                OptionLoadType = "Base",
                OptionToDate = new DateTime(2012, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified),
                PriceBookAppendix = "",
                ReferenceAreaName = "NPS",
                ReportDate = new DateTime(2011, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                Strike = 45,
                TemplateName = "NPXSYSALL"
            };
        }

        [Test]
        public void FailTestReportDateIsNotUnspecified()
        {
 
            IVolatilityService volatilitiservice = WCFClientUtil.GetVolatilityServiceProxy();
       
            try
            {
                Option option = GetDefaultOption();
                option.ReportDate = DateTime.MinValue;
                option.ReportDate = new DateTime(2011, 11, 16, 0, 0, 0, 0, DateTimeKind.Local);
                double[] result = volatilitiservice.GetImplicitVolatilitiesFromSurface(new[] { option });
                Console.WriteLine(result[0]);
            }
            catch (Exception ex)
            {
                const string expectedError = "Failed to return implicit volatilities: Variable 'ReportDate' must be unspecified kind";
                Assert.AreEqual(expectedError, ex.Message, "ReportDate not DateTimeKind.Unspecified Test failed");
                return;
            }
            Assert.Fail("Negative Test Failed");
        }


        [Test]
        public void FailTestOptionFromDateNotIsUnspecified()
        {

            IVolatilityService volatilitiservice = WCFClientUtil.GetVolatilityServiceProxy();

            try
            {
                Option option = GetDefaultOption();
                option.OptionFromDate = DateTime.MinValue;
                option.OptionFromDate = new DateTime(2012, 6, 1, 0, 0, 0, 0, DateTimeKind.Local);
                double[] result = volatilitiservice.GetImplicitVolatilitiesFromSurface(new[] { option });
            }
            catch (Exception ex)
            {
                const string expectedError = "Failed to return implicit volatilities: Variable 'OptionFrom' must be unspecified kind";
                Assert.AreEqual(expectedError, ex.Message, "ReportDate not DateTimeKind.Unspecified Test failed");
                return;
            }
            Assert.Fail("Negative Test Failed");
        }

        [Test]
        public void FailTestOptionToDateIsNotUnspecified()
        {

            IVolatilityService volatilitiservice = WCFClientUtil.GetVolatilityServiceProxy();

            try
            {
                Option option = GetDefaultOption();
                option.OptionToDate = DateTime.MinValue;
                option.OptionToDate = new DateTime(2012, 6, 30, 0, 0, 0, 0, DateTimeKind.Local);
                double[] result = volatilitiservice.GetImplicitVolatilitiesFromSurface(new[] { option });
            }
            catch (Exception ex)
            {
                const string expectedError = "Failed to return implicit volatilities: Variable 'OptionToDate' must be unspecified kind";
                Assert.AreEqual(expectedError, ex.Message, "ReportDate not DateTimeKind.Unspecified Test failed");
                return;
            }
            Assert.Fail("Negative Test Failed");
        }

        [Test]
        public void FailTestOptionExpiryIsNotUnspecified()
        {

            IVolatilityService volatilitiservice = WCFClientUtil.GetVolatilityServiceProxy();

            try
            {
                Option option = GetDefaultOption();
                option.OptionExpiry = DateTime.MinValue;
                option.OptionExpiry = new DateTime(2012, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc);
                double[] result = volatilitiservice.GetImplicitVolatilitiesFromSurface(new[] { option });
            }
            catch (Exception ex)
            {
                const string expectedError = "Failed to return implicit volatilities: Variable 'OptionExpiry' must be unspecified kind";
                Assert.AreEqual(expectedError, ex.Message, "ReportDate not DateTimeKind.Unspecified Test failed");
                return;
            }
            Assert.Fail("Negative Test Failed");
        }

        private const string TestFileVolatilitiesFromSurfaceTestFilePath = "TestFiles\\VolatilitiesFromSurface";

        private static readonly IEnumerable<string> TestFileVolatilitiesFromSurface = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFileVolatilitiesFromSurfaceTestFilePath);

        [Test, Timeout(1000 * 1000), TestCaseSource("TestFileVolatilitiesFromSurface")]
       public void GetImplicitVolatilitiesFromSurface(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                TestFileVolatilitiesFromSurfaceTestFilePath, testFile);

            VolatilitySurfaceTestCase volatilitySurfaceTest = TestXmlTool.Deserialize<VolatilitySurfaceTestCase>(testFilePath);

            IVolatilityService volatilityService = WCFClientUtil.GetVolatilityServiceProxy();

            double[] result = new double[] {};

            Option options = new Option
            {
                OptionExpiry = volatilitySurfaceTest.InputData.OptionExpiry,
                OptionFromDate = volatilitySurfaceTest.InputData.OptionFromDate,
                OptionLoadType = volatilitySurfaceTest.InputData.LoadType,
                OptionToDate = volatilitySurfaceTest.InputData.OptionToDate,
                PriceBookAppendix = volatilitySurfaceTest.InputData.PriceBookAppendix,
                ReferenceAreaName = volatilitySurfaceTest.InputData.ReferenceAreaName,
                ReportDate = volatilitySurfaceTest.InputData.ReportDate,
                Strike = volatilitySurfaceTest.InputData.Strike,
                TemplateName = volatilitySurfaceTest.InputData.TemplateName
            };
            try
            {
                result = volatilityService.GetImplicitVolatilitiesFromSurface(new[] { options });
                //foreach (var item in result)
                //{
                //    Console.WriteLine("<ExpectedVolatility Value=\"" + item + "\"/>");// + "\n";
                //}
                if (volatilitySurfaceTest.ExpectedResult.Result.ToUpper() == "FAILURE")
                {
                    string resultString="";
                    foreach (double item in result)
                    {
                        resultString += item;
                    }

                    Assert.Fail("Negativ volatilitySurfaceTest failed \n Expected : " +
                                volatilitySurfaceTest.ExpectedResult.ErrorMessage + "\nBut was :" + resultString);

                }

 


            }
            catch (FaultException ex)
            {
                //Assert.AreEqual("FAILURE", volatilitySurfaceTest.ExpectedResult.Result.ToUpper(), "Test failed");
                if (volatilitySurfaceTest.ExpectedResult.Result.ToUpper() == "SUCCESS")
                {
                    Assert.Fail("Possitiv volatilitySurfaceTest failed with exception :\n"+ex.Message);
                }
                else
                {

                    Assert.AreEqual("FAILURE", volatilitySurfaceTest.ExpectedResult.Result.ToUpper(), "Test failed");
                    Assert.AreEqual(volatilitySurfaceTest.ExpectedResult.ErrorMessage, ex.Message,
                        "Other exception was caught: " + ex.Message);
                    return;
                }

            }
            catch (Exception e)
            {
                Assert.Fail("Unhandled Exception \n"+e.Message);
            }

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, volatilitySurfaceTest.ExpectedValues.Length, "Test GetImplicitVolatilitiesFromSurface can only have one expected Volatility");
            double actual = result[0];
            double expected = volatilitySurfaceTest.ExpectedValues[0].Value;

            if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
            {
                double relativeError = (actual / expected) - 1;
                string errorMessage = "Volatility did not match. \n" +
                                       "   Expected = " + expected + "\n" +
                                       "   Actual   = " + actual;
                //Assert value
              //  Console.WriteLine(actual);
                Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE, errorMessage);
            }
        
        }

        private const string TestFileVolatilitiesForStripFromSurfaceFilePath =
            "TestFiles\\VolatilitiesForStripFromSurface";
        private static readonly IEnumerable<string> TestFileVolatilitiesForStripFromSurface = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFileVolatilitiesForStripFromSurfaceFilePath);

       [Test, Timeout(1000 * 1000), TestCaseSource("TestFileVolatilitiesForStripFromSurface")]
        public void GetImplicitVolatilitiesForStripFromSurface(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(),
            TestFileVolatilitiesForStripFromSurfaceFilePath, testFile);

            VolatilitySurfaceTestCase volatilitySurfaceTest = TestXmlTool.Deserialize<VolatilitySurfaceTestCase>(testFilePath);


 
           IVolatilityService volatilitiservice = WCFClientUtil.GetVolatilityServiceProxy();

            OptionStripDescription criteria = new OptionStripDescription();
            {
                criteria.ExerciseTimeOfDay = volatilitySurfaceTest.InputData.ExerciseTimeOfDay; 
                criteria.OptionLoadType = volatilitySurfaceTest.InputData.LoadType;
                criteria.PriceBookAppendix = volatilitySurfaceTest.InputData.PriceBookAppendix;
                criteria.ReferenceAreaName = volatilitySurfaceTest.InputData.ReferenceAreaName;
                criteria.ReportDate = volatilitySurfaceTest.InputData.ReportDate;
                criteria.Resolution = volatilitySurfaceTest.InputData.Resolution;
                criteria.Strike = volatilitySurfaceTest.InputData.Strike;
                criteria.StripFromDate = volatilitySurfaceTest.InputData.OptionFromDate;
                criteria.StripToDate = volatilitySurfaceTest.InputData.OptionToDate;
                criteria.TemplateName = volatilitySurfaceTest.InputData.TemplateName;
                
            };

           try
           {
               OptionStripVolatility[] result = volatilitiservice.GetImplicitVolatilitiesForStripFromSurface(criteria);
                string res = "";
                foreach (OptionStripVolatility item in result)
                {
                    res += "<ExpectedVolatility FromDateTime=\"" + item.FromDateTime.ToString("yyyy-MM-ddTHH:mm:ss") + "\" Value=\"" + item.Volatility + "\"/>" + "\n";

                   // Console.WriteLine(item.FromDateTime + ";" + item.Volatility);
                }

             //   Console.WriteLine(res);

                if (volatilitySurfaceTest.ExpectedResult.Result.ToUpper() == "FAILURE")
               {
                   string resultString = "";
                   foreach (OptionStripVolatility item in result)
                   {
                       resultString += item.FromDateTime + ";" + item.Volatility + "\n";
                   }

                   Assert.Fail("Negativ volatilitySurfaceTest failed \n Expected : " +
                               volatilitySurfaceTest.ExpectedResult.ErrorMessage + "\nBut was :" + resultString);

               }

               Assert.AreEqual("SUCCESS", volatilitySurfaceTest.ExpectedResult.Result.ToUpper(),
                   "Negativ volatilitySurfaceTest failed");

               ExpectedVolatility[] expectedVolatilities = volatilitySurfaceTest.ExpectedValues;

               Assert.AreEqual(expectedVolatilities.Length, result.Length, "Number of Expected Records do not match:");

               for (int i = 0; i < expectedVolatilities.Length; i++)
               {
                   Assert.AreEqual(expectedVolatilities[i].FromDateTime,result[i].FromDateTime,"Record number "+(i+1)+" does not match FromDateTime");

                   double actual = result[i].Volatility;
                   double expected = volatilitySurfaceTest.ExpectedValues[i].Value;

                   if (Math.Abs(actual) >= 1e-8 || Math.Abs(expected) >= 1e-8)
                   {
                       double relativeError = (actual / expected) - 1;
                       string errorMessage = "The actual value did not match expected value " +
                                             actual + " vs " + expected;
                       //Assert value
                       Assert.AreEqual(0, relativeError, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE,"Error on DateTime "+expectedVolatilities[i].FromDateTime+" : "+ errorMessage);
                   }
               }
           }
           catch (FaultException ex)
           {
               //Assert.AreEqual("FAILURE", volatilitySurfaceTest.ExpectedResult.Result.ToUpper(), "Test failed");
               if (volatilitySurfaceTest.ExpectedResult.Result.ToUpper() == "SUCCESS")
               {
                   Assert.Fail("Possitiv volatilitySurfaceTest failed \n"+ex.Message);
               }
               else
               {

                   Assert.AreEqual("FAILURE", volatilitySurfaceTest.ExpectedResult.Result.ToUpper(), "Test failed");
                   Assert.AreEqual(volatilitySurfaceTest.ExpectedResult.ErrorMessage, ex.Message,
                       "Other exception was caught: " + ex.Message);
               }
           }
           catch (Exception e)
           {
               Assert.Fail("Unhandled Exception \n" + e.Message);
           }



        }

       
    }
}
