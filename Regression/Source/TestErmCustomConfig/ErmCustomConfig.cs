using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestErmCustomConfig
{
    [TestFixture]
    public class ErmCustomConfig
    {


        [TestFixtureSetUp]
        public void SetUp()
        {
            ConfigurationTool.MissingRealizedDataStrategy = "ThrowException";
            EcmTestUtil.ExportAllTransactionsToDwh();

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
        }

        [Test, TestCaseSource("NewTestMonthFiles")]
        [Category("ERM_New")]
        public void TestERM_newCustomConfig(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Day";

            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NewTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            
            SetConfiguration(test.Setup.ElvizConfigurations);
            
            ErmTestUtil.DoTestERM(test, "ERM_New", newVersion_calculations, newVersion_verification, false, resolution,true);
        }

        private static readonly IEnumerable<string> NewTestMonthFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("NewTestFiles");

        [Test, TestCaseSource("NewTestMonthFiles")]
        [Category("ERM_New_Parallel")]
        public void TestERM_newInParallelCustomConfig(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Day";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NewTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            SetConfiguration(test.Setup.ElvizConfigurations);
            
            ErmTestUtil.DoTestERM(test, "ERM_New_Parallel", newVersion_calculations, newVersion_verification, true, resolution,true);
        }

        private static readonly IEnumerable<string> ExposureByMonthTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("ExposureByMonthTestFiles");

        [Test, TestCaseSource("ExposureByMonthTestFiles")]
        [Category("ERM_New_Parallel_Month")]
        public void TestExposureByMonth_InParallelCustomConfig(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Month";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByMonthTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            SetConfiguration(test.Setup.ElvizConfigurations);
            
            ErmTestUtil.DoTestERM(test, "ERM_New_Month_Paralell", newVersion_calculations, newVersion_verification, true, resolution,true);
        }

      
        private static readonly IEnumerable<string> ExposureByHourTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFilesFiltred("ExposureByHourTestFiles");

        [Test, TestCaseSource("ExposureByHourTestFiles")]
        [Category("ERM_New_Parallel_Hour")]
        public void TestExposureByHour_InParallelCustomConfig(string testFile)
        {
            const bool newVersionCalculations = true;
            const bool newVersionVerification = true;
            const string resolution = "Hour";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByHourTestFiles\\" + testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            SetConfiguration(test.Setup.ElvizConfigurations);

            ErmTestUtil.DoTestERM(test, "ERM_New_Hour_Paralell", newVersionCalculations, newVersionVerification, true, resolution,true);
        }

        private static readonly IEnumerable<string> TestFilesERM = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        [Test, TestCaseSource("TestFilesERM")]
        [Category("TestERM")]
        public void TestErmCustomConfig(string testFile)
        {
            bool newVersion_calculations = false;
            bool newVersion_verification = false;
            string resolution = "Day";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            SetConfiguration(test.Setup.ElvizConfigurations);
            
            ErmTestUtil.DoTestERM(test, "ERM_Old", newVersion_calculations, newVersion_verification, false, resolution,true);
        }

        private void SetConfiguration(ElvizConfiguration[] configurations)
        {
            if (configurations == null || configurations.Length == 0)
            {
                Assert.Fail("Test expected to contain Custom Elviz Configuration");
            }

            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.UpdateConfiguration(configurations);
      
        }

    }
}
