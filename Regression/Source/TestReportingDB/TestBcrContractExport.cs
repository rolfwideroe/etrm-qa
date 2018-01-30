using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.ReportingDbTimeSeries;
using NUnit.Framework;

namespace TestReportingDB
{
    [TestFixture]
    public class TestBcrContractExport
    {
        private const string BcrContractExportFolder = "TestFiles\\BCRContractExport\\";
        private static readonly IEnumerable<string> TestFilesFolderBcrContractExport = TestCasesFileEnumeratorByFolder.TestCaseFiles(BcrContractExportFolder);
        

        [OneTimeSetUp]
        public void Setup()
        {
            //ReportDbTestsSetup.LoadStaticData();

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ElvizConfigurationTool configurationTool = new ElvizConfigurationTool();
            configurationTool.RevertAllConfigurationsToDefault();

        }

        [Test, TestCaseSource("TestFilesFolderBcrContractExport"), Timeout(200 * 1000)]
        public void TestBcrContractExportTest(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), BcrContractExportFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
       
            ReportDbTestUtil.TestBcrContractExport(testCase,filePath);
        }
    }
}
