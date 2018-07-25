using System;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TestElvizUpdateTool;
using TestElvizUpdateTool.Helpers;

namespace TestElvizUpdateTools.Tests
{
    [TestClass]
    public class ElvizTests
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase
            .GetCurrentMethod()
            .DeclaringType);


        [TestMethod]
        public void ManageWindowsDirectoriesTest()
        {
            var fullTestCaseFilePath = @"C:\Source\qa-etrm\Regression\Bin\TestElvizUpdateTool";

            var manageWindowsDirectories =
                new ManageWindowsDirectories(fullTestCaseFilePath, @"C:\MyDocumentsCopy", true);

            manageWindowsDirectories.Replenish();

        }

        [TestMethod]
        public void ReportDateHandlerIsDaylightTimeTest()
        {
            var reportDate = DateTime.Parse("Mar 25, 2018");
            var reportDateHandler = new ReportDateHandler(reportDate);
            Assert.IsTrue(reportDateHandler.IsDayLightTime());

            reportDate = DateTime.Parse("Oct 28, 2018");
            reportDateHandler = new ReportDateHandler(reportDate);
            Assert.IsTrue(reportDateHandler.IsDayLightTime());
        }

        [TestMethod]
        public void ReportDateHandlerIgnoreWeekendTest()
        {
            var adjustToSunday = (int) DateTime.Now.DayOfWeek * -1;
            var reportDate = DateTime.Now.AddDays(adjustToSunday);

            var reportDateHandler = new ReportDateHandler(reportDate);

            Assert.AreEqual(reportDateHandler.IgnoreWeekends(), reportDate.AddDays(-2));


            reportDate = DateTime.Now.AddDays(adjustToSunday - 1);

            reportDateHandler = new ReportDateHandler(reportDate);

            Assert.AreEqual(reportDateHandler.IgnoreWeekends(), reportDate.AddDays(-1));
        }


        [TestMethod]
        public void ShouldlyAssertsOnJobs()
        {
            var testcaseName = @"TestFiles\EUTJobs.xml";

            var eutJobs = new EutJobs(testcaseName);

            //Assert.AreEqual(eutJobs.TestCaseJobItemsList.ToList().Count, 16);
            //eutJobs.TestCaseJobItemsList.ToList().Count.ShouldBe(16);

            //eutJobs.ShouldBeOfType<string>();
            //eutJobs.ShouldBeOfType<EutJobs>();

            //eutJobs.ShouldBeAssignableTo(typeof(string), "Crapped out because it wasnt correct type");
            //eutJobs.ShouldBeAssignableTo(typeof(EutJobs), "Crapped out because it wasnt correct type");
        }

        [TestMethod]
        public void CompareObjectsOnJobs()
        {
            var testcaseName = @"TestFiles\EUTJobs.xml";
            var testcaseName2 = @"TestFiles\EUTJobsChanged.xml";

            var eutJobs = new EutJobs(testcaseName);
            var eutJobsCopy = eutJobs;
            var eutJobsCompare = new EutJobs(testcaseName);

            var eutJobsChanged = new EutJobs(testcaseName2);

            eutJobs.ShouldBeSameAs(eutJobsCopy);
            eutJobs.ShouldNotBeSameAs(eutJobsCompare);



            var compare = new CompareLogic {Config = {MaxDifferences = 3}};

            var comparisonResult = compare.Compare(eutJobs, eutJobsCompare);
            comparisonResult.AreEqual.ShouldBe(true);


            var comparisonResult2 = compare.Compare(eutJobs, eutJobsChanged);

            comparisonResult2.Config.MaxDifferences = 3;

            if (comparisonResult2.Differences.Count > 3)
            {
                var dString = comparisonResult2.DifferencesString;

                var maxDiffs = comparisonResult2.ExceededDifferences;
            }



            comparisonResult2.AreEqual.ShouldBe(true, comparisonResult2.DifferencesString);

        }
    }
}
