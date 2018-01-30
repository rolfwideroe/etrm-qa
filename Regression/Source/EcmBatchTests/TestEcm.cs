using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestEcmBatch
{
    [TestFixture]
    public class TestEcm
    {
	    private static readonly IEnumerable<string> TestFilesECM = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");
		private static readonly IEnumerable<string> TestFilesMutablePosMon = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles_MutablePosMon");

		[OneTimeSetUp]
        public void SetUp()
        {
            ConfigurationTool.MissingRealizedDataStrategy = "ThrowException";
            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
            EcmTestUtil.ExportAllTransactionsToDwh();
        }

	    [OneTimeTearDown]
        public void TearDown()
        {
            ElvizConfigurationTool utility =new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
        }

	    [Test, TestCaseSource(nameof(TestFilesECM))]
        public void TestECM(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);

            BatchTestFile test=BatchTestFileParser.DeserializeXml(testFilePath);
            EcmTestUtil.TestECM(test,true);
        }

		[Test, TestCaseSource(nameof(TestFilesMutablePosMon))]
		public void TestECM_MutablePosMon(string testFile)
		{
			string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles_MutablePosMon\\" + testFile);

			BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
			EcmTestUtil.TestMutablePosMon(test, true);
		}
	}
}
