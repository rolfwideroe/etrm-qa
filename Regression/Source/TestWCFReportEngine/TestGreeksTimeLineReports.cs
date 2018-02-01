using ElvizTestUtils;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.ReportEngineTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestWCFReportEngine
{
    [TestFixture]
    public class TestGreeksTimeLineReports
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test,Timeout(10*1000)]
        public void TestGetPredefinedSettingSet()
        {
            string dateNowasXmlEncodedValue = TestXmlTool.ConvertToXmlDateTimeString(DateTime.Now.Date);  

            Setting[] expectedSettings =
            {
                new Setting() {SettingId = "GroupPositionsBy", XmlEncodedValue = "Portfolio", IsNotSet = false},
                new Setting() {SettingId = "PosCalcMethod", XmlEncodedValue = "NPVWA", IsNotSet = false},
                new Setting() {SettingId = "ReportCurrency", XmlEncodedValue = "EUR", IsNotSet = false},
                new Setting() {SettingId = "ReportDate", XmlEncodedValue = dateNowasXmlEncodedValue, IsNotSet = false},
                new Setting() {SettingId = "TimeResolution", XmlEncodedValue = "Month", IsNotSet = false},
                new Setting() {SettingId = "TransactionFilter", XmlEncodedValue = "-1", IsNotSet = false},
                new Setting() {SettingId = "UseEndDate", XmlEncodedValue = "false", IsNotSet = false},
                new Setting() {SettingId = "UseEndOfDayReporting", XmlEncodedValue = "true", IsNotSet = false},
                new Setting() {SettingId = "UseVolatilitySurface", XmlEncodedValue = "false", IsNotSet = false}
            };

            IInternalReportingApiService service = WCFClientUtil.GetInternalReportingServiceProxy();
            Setting[] settings = service.GetPredefinedSettingsSet("GreeksOnATimelineReport", "Default");

            Setting[] orderedSettings=settings.OrderBy(x => x.SettingId).ToArray();

            Assert.AreEqual(expectedSettings.Length,orderedSettings.Length,"Number of Expected Settings did not match");

            for (int i = 0; i < expectedSettings.Length; i++)
            {
                Assert.AreEqual(expectedSettings[i].SettingId,orderedSettings[i].SettingId,"SettingId did not match");
                Assert.AreEqual(expectedSettings[i].XmlEncodedValue, orderedSettings[i].XmlEncodedValue, "XmlEncodedValue did not match for Setting Id : "+expectedSettings[i].SettingId);
                Assert.AreEqual(expectedSettings[i].IsNotSet, orderedSettings[i].IsNotSet, "IsNotSet did not match for Setting Id :"+expectedSettings[i].SettingId);
            }
        }

        private const string GreeksOnATimeLineFolder = "TestFiles\\GreeksOnATimeLine";
        private static readonly IEnumerable<string> TestFileGreeksOnATimeLine = TestCasesFileEnumeratorByFolder.TestCaseFiles(GreeksOnATimeLineFolder);
        [Test, TestCaseSource("TestFileGreeksOnATimeLine"),Timeout(60*1000)]
        public void TestGreeksOnATimeLine(string testFilePath)
        {
            
            TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilePath, GreeksOnATimeLineFolder);
            if (string.IsNullOrEmpty(test.ExpectedValues.ExceptionErrorMessage))
            {
				ReportEngineTestUtils.TestAllArtifacts(test, "errorTable");
            }
        }
	    
    }
}
