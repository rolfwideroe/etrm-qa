using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.ReportingDbTimeSeries;
using NUnit.Framework;

namespace TestReportingDB
{

    [TestFixture]
    public class TestTimeSeries
    {

        [OneTimeSetUp]
        public void Setup()
        {
            ReportDbTestsSetup.LoadStaticData();
        }

        // [TestFixtureTearDown]
        public void TearDown()
        {
            ElvizConfigurationTool configurationTool = new ElvizConfigurationTool();
            configurationTool.RevertAllConfigurationsToDefault();
        }

        private const string MWhFolder = "TestFiles\\MWh\\";
        private const string MWFolder = "TestFiles\\MW\\";
        private const string ContractValueFolder = "TestFiles\\ContractValue\\";
        private const string ContractPriceFolder = "TestFiles\\ContractPrice\\";
        private const string VolumeFolder = "TestFiles\\Volume\\";
        private const string AccruedPLFolder = "TestFiles\\AccruedPL\\";
        private const string AccruedPLNOKFolder = "TestFiles\\AccruedPLNOK\\";
        private const string AccruedCFFolder = "TestFiles\\AccruedCF\\";
        private const string SettlementFolder = "TestFiles\\Settlement\\";
        private const string OptionsFolder = "TestFiles\\Options\\";
        private const string FeesFolder = "TestFiles\\Fees\\";
        private const string ZippedBigFilesFolder = "TestFiles\\ZippedBigFiles\\";


        private static readonly IEnumerable<string> TestFilesFolderMWh = TestCasesFileEnumeratorByFolder.TestCaseFiles(MWhFolder);
        private static readonly IEnumerable<string> TestFilesFolderMW = TestCasesFileEnumeratorByFolder.TestCaseFiles(MWFolder);
        private static readonly IEnumerable<string> TestFilesFolderContractValue = TestCasesFileEnumeratorByFolder.TestCaseFiles(ContractValueFolder);
        private static readonly IEnumerable<string> TestFilesFolderContractPrice = TestCasesFileEnumeratorByFolder.TestCaseFiles(ContractPriceFolder);
        private static readonly IEnumerable<string> TestFilesFolderVolume = TestCasesFileEnumeratorByFolder.TestCaseFiles(VolumeFolder);
        private static readonly IEnumerable<string> TestFilesFolderAccruedPL = TestCasesFileEnumeratorByFolder.TestCaseFiles(AccruedPLFolder);
        private static readonly IEnumerable<string> TestFilesFolderAccruedPLNOK = TestCasesFileEnumeratorByFolder.TestCaseFiles(AccruedPLNOKFolder);
        private static readonly IEnumerable<string> TestFilesFolderAccruedCF = TestCasesFileEnumeratorByFolder.TestCaseFiles(AccruedCFFolder);
        private static readonly IEnumerable<string> TestFilesFolderSettlement = TestCasesFileEnumeratorByFolder.TestCaseFiles(SettlementFolder);
        private static readonly IEnumerable<string> TestFilesFolderOptions = TestCasesFileEnumeratorByFolder.TestCaseFiles(OptionsFolder);
        private static readonly IEnumerable<string> TestFilesFolderFees = TestCasesFileEnumeratorByFolder.TestCaseFiles(FeesFolder);
        private static readonly IEnumerable<string> TestFilesFolderZippedBigFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles(ZippedBigFilesFolder);



        [Test, TestCaseSource(nameof(TestFilesFolderMWh)), Timeout(300 * 1000)]
        public void TestMWh(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), MWhFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderMW)), Timeout(300 * 1000)]
        public void TestMW(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), MWFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderAccruedPL)), Timeout(300 * 1000)]
        public void TestAccruedPL(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), AccruedPLFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderAccruedPLNOK)), Timeout(300 * 1000)]
        public void TestAccruedPLNOK(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), AccruedPLNOKFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderAccruedCF)), Timeout(300 * 1000)]
        public void TestAccruedCF(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), AccruedCFFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderContractValue)), Timeout(200 * 1000)]
        public void TestContractValue(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ContractValueFolder + fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderContractPrice)), Timeout(200 * 1000)]
        public void TestContractPrice(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ContractPriceFolder + fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderVolume)), Timeout(200 * 1000)]
        public void TestVolume(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), VolumeFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderSettlement)), Timeout(200 * 1000)]
        public void TestSettlement(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), SettlementFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderOptions)), Timeout(200 * 1000)]
        public void TestOptions(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), OptionsFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource(nameof(TestFilesFolderFees)), Timeout(200 * 1000)]
        public void TestFees(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), FeesFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestFeesTimeSeries(testCase, filePath);
        }

        [Test, TestCaseSource("TestFilesFolderZippedBigFiles"), Timeout(300 * 1000)]
        public void TestZippedBigFiles(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ZippedBigFilesFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.DeserializeFromZip<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);

        }

    }
}
