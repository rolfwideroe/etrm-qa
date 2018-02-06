using System;
using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestErmCustomConfig
{
    [TestFixture]
    public class TestErmCustomConfigDwhExportOnly
    {

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }


        private static readonly IEnumerable<string> NewTestMonthFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("NewTestFiles");

        [Test, TestCaseSource("NewTestMonthFiles")]
        [Category("DwhExportOnly")]
        public void TestERM_newInParallelDwhExportOnly(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Day";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NewTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test, "ERM_New_Parallel", newVersion_calculations, newVersion_verification, true, resolution, false);
        }

        private static readonly IEnumerable<string> ExposureByMonthTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("ExposureByMonthTestFiles");

        [Test, TestCaseSource("ExposureByMonthTestFiles")]
        [Category("DwhExportOnly")]
        public void TestExposureByMonth_InParallelDwhExportOnly(string testFile)
        {
            bool newVersion_calculations = true;
            bool newVersion_verification = true;
            string resolution = "Month";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByMonthTestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test, "ERM_New_Month_Paralell", newVersion_calculations, newVersion_verification, true, resolution, false);
        }

        private static readonly IEnumerable<string> ExposureByHourTestFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles("ExposureByHourTestFiles");

        [Test, TestCaseSource("ExposureByHourTestFiles")]
        [Category("DwhExportOnly")]
        public void TestExposureByHour_InParallelDwhExportOnly(string testFile)
        {
            const bool newVersionCalculations = true;
            const bool newVersionVerification = true;
            const string resolution = "Hour";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExposureByHourTestFiles\\" + testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);


            ErmTestUtil.DoTestERM(test, "ERM_New_Hour_Paralell", newVersionCalculations, newVersionVerification, true, resolution, false);
        }

        private static readonly IEnumerable<string> TestFilesERM = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        [Test, TestCaseSource("TestFilesERM")]
        [Category("DwhExportOnly")]
        public void TestERMDwhExportOnly(string testFile)
        {
            bool newVersion_calculations = false;
            bool newVersion_verification = false;
            string resolution = "Day";
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles\\" + testFile);
            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);
            ErmTestUtil.DoTestERM(test, "ERM_Old", newVersion_calculations, newVersion_verification, false, resolution, false);
        }



    }
}
