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
    public class TestKeyValues
    {
        private static readonly IEnumerable<string> TestFilesFolderKeyValues = TestCasesFileEnumeratorByFolder.TestCaseFiles(KeyValuesFolder);
        private static readonly IEnumerable<string> TestFilesFolderKeyValues_Options = TestCasesFileEnumeratorByFolder.TestCaseFiles(KeyValuesFolder_Options);
        private const string KeyValuesFolder = "TestFiles\\KeyValues\\";
		private const string KeyValuesFolder_Options = "TestFiles\\KeyValues_Options\\";


		[OneTimeSetUp]
        public void Setup()
        {
            ReportDbTestsSetup.LoadStaticData();
            

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ElvizConfigurationTool configurationTool = new ElvizConfigurationTool();
            configurationTool.RevertAllConfigurationsToDefault();

        }

        [Test, TestCaseSource(nameof(TestFilesFolderKeyValues)), Timeout(200 * 1000)]
        public void TestMultippleKeyValues(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), KeyValuesFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
       
            ReportDbTestUtil.TestKeyValues(testCase,filePath);
        }

		[Test, TestCaseSource(nameof(TestFilesFolderKeyValues_Options)), Timeout(200 * 1000)]
		public void TestKeyValuesForOptions(string fileName)
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), KeyValuesFolder_Options, fileName);
			ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);

			ReportDbTestUtil.TestKeyValues(testCase, filePath);
		}

	}
}
