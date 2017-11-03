using ElvizTestUtils;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.ReportEngineTests;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestWCFReportEngine
{
    [TestFixture]
    public class TestNasdaqClearingReports
    {

        [Test,Timeout(10*1000)]
        public void TestNasdaqReportPreDefinedSettings()
        {
            InternalReportingApiServiceClient serviceClient = WCFClientUtil.GetInternalReportingServiceProxy();

            Setting[] s=serviceClient.GetPredefinedSettingsSet("NasdaqClearingReport", "Default");
        }

        private static readonly IEnumerable<string> TestFilesFutureMarkToMarket = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\ExchangeMarketChecklist\\FutureMarkToMarket");

        [Test, Timeout(60*1000),TestCaseSource("TestFilesFutureMarkToMarket")]
        public void TestFutureMarkToMarket(string testFilepath)
        {
            TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilepath,"TestFiles\\ExchangeMarketChecklist\\FutureMarkToMarket");

	        if (string.IsNullOrEmpty(test.ExpectedValues.ExceptionErrorMessage))
	        {
		        ReportEngineTestUtils.TestAllArtifacts(test);
	        }
        }

	    private static readonly IEnumerable<string> TestFilesContractsInDelivery = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\ExchangeMarketChecklist\\ContractsInDelivery");

        [Test, Timeout(60 * 1000),TestCaseSource("TestFilesContractsInDelivery")]
        public void TestContractsInDelivery(string testFilepath)
        {
            TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilepath, "TestFiles\\ExchangeMarketChecklist\\ContractsInDelivery");
	        if (string.IsNullOrEmpty(test.ExpectedValues.ExceptionErrorMessage))
	        {
		        ReportEngineTestUtils.TestAllArtifacts(test);
	        }
        }

        private static readonly IEnumerable<string> TestFilesContractsAccruedMarketValue = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles\\ExchangeMarketChecklist\\ContractsAccruedMarketValue");

        [Test, Timeout(60 * 1000),TestCaseSource("TestFilesContractsAccruedMarketValue")]
        public void TestContractsAccruedMarketValue(string testFilepath)
        {
            TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilepath, "TestFiles\\ExchangeMarketChecklist\\ContractsAccruedMarketValue");
	        if (string.IsNullOrEmpty(test.ExpectedValues.ExceptionErrorMessage))
	        {
		        ReportEngineTestUtils.TestAllArtifacts(test);
	        }
        }
     
    }
}
