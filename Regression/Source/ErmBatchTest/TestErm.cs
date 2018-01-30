using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestErmBatch
{
	[TestFixture]
	public class TestErm
	{
        //   readonly ElvizConfiguration[] configurations = { new ElvizConfiguration("ForwardExchangeSource", "Viz") };

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
            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
        }

        [Test, TestCaseSource("NewTestMonthFiles")]
        [Category("ERM_New")]
		public void TestERM_new(string testFile)
		{
			bool newVersion_calculations = true;
			bool newVersion_verification = true;
		    string resolution = "Day";
			string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NewTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test, "ERM_New", newVersion_calculations, newVersion_verification, false, resolution,true);
		}

        private static readonly IEnumerable<string> NewTestMonthFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("NewTestFiles");

        [Test, TestCaseSource("NewTestMonthFiles")]
        [Category("ERM_New_Parallel")]
		public void TestERM_newInParallel(string testFile)
		{
			bool newVersion_calculations = true;
			bool newVersion_verification = true;
            string resolution = "Day";
			string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NewTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
			ErmTestUtil.DoTestERM(test,"ERM_New_Parallel", newVersion_calculations, newVersion_verification, true,resolution,true);
		}

        private static readonly IEnumerable<string> ExposureByMonthTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("ExposureByMonthTestFiles");

        [Test, TestCaseSource("ExposureByMonthTestFiles")]
        [Category("ERM_New_Parallel_Month")]
        public void TestExposureByMonth_InParallel(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Month";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByMonthTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test,"ERM_New_Month_Paralell", newVersion_calculations, newVersion_verification, true,resolution,true);
        }

        private static readonly IEnumerable<string> ExposureByHourTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("ExposureByHourTestFiles");

        [Test, TestCaseSource("ExposureByHourTestFiles")]
        [Category("ERM_New_Parallel_Hour")]
        public void TestExposureByHour_InParallel(string testFile)
        {
            const bool newVersionCalculations = true;
            const bool newVersionVerification = true;
            const string resolution = "Hour";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByHourTestFiles\\" + testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
                   
  
            ErmTestUtil.DoTestERM(test, "ERM_New_Hour_Paralell", newVersionCalculations, newVersionVerification, true, resolution,true);
        }

        private static readonly IEnumerable<string> TestFilesERM = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        [Test, TestCaseSource("TestFilesERM")]
        [Category("TestERM")]
		public void TestERM(string testFile)
		{
			bool newVersion_calculations = false;
			bool newVersion_verification = false;
            string resolution = "Day";
			string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test,"ERM_Old", newVersion_calculations, newVersion_verification, false, resolution,true);
		}

	 
	
	}
}
