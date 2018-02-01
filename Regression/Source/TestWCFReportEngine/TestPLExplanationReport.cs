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
	public class TestPLExplanationReport
	{
	    [OneTimeSetUp]
	    public void RunBeforeAnyTests()
	    {
	        Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
	    }

        [Test,Timeout(10*1000)]
		public void TestGetPredefinedSettingSet()
		{
			string dateNowAsXmlEncodedValue = TestXmlTool.ConvertToXmlDateTimeString(DateTime.Now.Date);
			string dateYesterdayasXmlEncodedValue = TestXmlTool.ConvertToXmlDateTimeString(DateTime.Now.AddDays(-1).Date);

			List<Setting> expectedSettings = new List<Setting>
            {
				new Setting() {SettingId = "CompareDate", XmlEncodedValue = dateNowAsXmlEncodedValue, IsNotSet = false},
				new Setting() {SettingId = "ReportDate", XmlEncodedValue = dateYesterdayasXmlEncodedValue, IsNotSet = false},
				new Setting() {SettingId = "TransactionFilter", XmlEncodedValue = "-1", IsNotSet = false},
				new Setting() {SettingId = "ReportCurrency", XmlEncodedValue = "EUR", IsNotSet = false},
				new Setting() {SettingId = "ShowValuesAs", XmlEncodedValue = "Total", IsNotSet = false},
                new Setting() {SettingId = "GroupPositionsBy", XmlEncodedValue = "Portfolio", IsNotSet = false},
				new Setting() {SettingId = "UseVolatilitySurface", XmlEncodedValue = "false", IsNotSet = false},
				new Setting() {SettingId = "UseEndOfDayReporting", XmlEncodedValue = "true", IsNotSet = false},
				new Setting() {SettingId = "UseActualDateSpotRates", XmlEncodedValue = "true", IsNotSet = false},
				new Setting() {SettingId = "AggregateStructuredDeals", XmlEncodedValue = "false", IsNotSet = false},
                new Setting() {SettingId = "IntrinsicEvaluation", XmlEncodedValue = "false", IsNotSet = false},
                new Setting() {SettingId = "ReportPerCurve", XmlEncodedValue = "false", IsNotSet = false},
                new Setting() {SettingId = "ReportPerCurrency", XmlEncodedValue = "false", IsNotSet = false},
            };
			expectedSettings = expectedSettings.OrderBy(x => x.SettingId).ToList();

			IInternalReportingApiService service = WCFClientUtil.GetInternalReportingServiceProxy();
			List<Setting> actualSettings = service.GetPredefinedSettingsSet("ProfitLossExplanationReport", "Default").ToList();
			actualSettings = actualSettings.OrderBy(x => x.SettingId).ToList();

			Assert.AreEqual(expectedSettings.Count, actualSettings.Count, "Number of Expected Settings did not match");
			foreach (Setting expectedSetting in expectedSettings)
			{
				Setting match = actualSettings.SingleOrDefault(x => x.SettingId.Equals(expectedSetting.SettingId));
				if (match == null)
					Assert.Fail("Expected setting not found: " + expectedSetting.SettingId);
			}
		}

		private const string PLExplanationFolder = "TestFiles\\PLExplanation";
		private static readonly IEnumerable<string> TestFilePLExplanation = TestCasesFileEnumeratorByFolder.TestCaseFiles(PLExplanationFolder);
		[Test, TestCaseSource("TestFilePLExplanation"),Timeout(120 * 1000)]
		public void TestPLExplanation(string testFilePath)
		{
            Console.WriteLine("Testing : "+testFilePath);
			TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilePath, PLExplanationFolder);
			ReportEngineTestUtils.TestAllArtifacts(test, "errorTable");
		}

		private const string PLExplanationPerCurveFolder = "TestFiles\\PLExplanation\\PLExplanationPerCurve";
		private static readonly IEnumerable<string> TestFilePLExplanationPerCurve = TestCasesFileEnumeratorByFolder.TestCaseFiles(PLExplanationPerCurveFolder);
		[Test, TestCaseSource(nameof(TestFilePLExplanationPerCurve)), Timeout(120 * 1000)]
		public void TestPLExplanationPerCurve(string testFilePath)
		{
			Console.WriteLine("Testing : " + testFilePath);
			TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilePath, PLExplanationPerCurveFolder);
			ReportEngineTestUtils.TestAllArtifacts(test, "errorTable");
		}

		private const string PLExplanationPerCurrencyFolder = "TestFiles\\PLExplanation\\PLExplanationPerCurrency";
		private static readonly IEnumerable<string> TestFilePLExplanationPerCurrency = TestCasesFileEnumeratorByFolder.TestCaseFiles(PLExplanationPerCurrencyFolder);
		[Test, TestCaseSource(nameof(TestFilePLExplanationPerCurrency)), Timeout(120 * 1000)]
		public void TestPLExplanationPerCurrency(string testFilePath)
		{
			Console.WriteLine("Testing : " + testFilePath);
			TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilePath, PLExplanationPerCurrencyFolder);
			ReportEngineTestUtils.TestAllArtifacts(test, "errorTable");
		}

	}
}
