using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestElvizUpdateTool.Helpers;

namespace TestElvizUpdateTools.Tests
{
    [TestClass]
    public class ElvizTests
    {
        [TestMethod]
        public void ManageWindowsDirectoriesTest()
        {
            var fullTestCaseFilePath = @"C:\Source\qa-etrm\Regression\Bin\TestElvizUpdateTool";

            var manageWindowsDirectories = new ManageWindowsDirectories(fullTestCaseFilePath, @"C:\MyDocumentsCopy", true);

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
            var adjustToSunday = (int)DateTime.Now.DayOfWeek * -1;
            var reportDate = DateTime.Now.AddDays(adjustToSunday);

            var reportDateHandler = new ReportDateHandler(reportDate);

            Assert.AreEqual(reportDateHandler.IgnoreWeekends(), reportDate.AddDays(-2));


            reportDate = DateTime.Now.AddDays(adjustToSunday - 1);

            reportDateHandler = new ReportDateHandler(reportDate);

            Assert.AreEqual(reportDateHandler.IgnoreWeekends(), reportDate.AddDays(-1));
        }
    }
}
