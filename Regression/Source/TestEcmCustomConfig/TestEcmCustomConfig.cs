using System;
using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestEcmCustomConfig
{
    [TestFixture]
    public class EcmCustomConfig
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [SetUp]
        public void SetUp()
        {
            ConfigurationTool.MissingRealizedDataStrategy = "ThrowException";
            EcmTestUtil.ExportAllTransactionsToDwh();
        }

        [TearDown]
        public void TearDown()
        {
            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.RevertAllConfigurationsToDefault();
        }

        private const string TestFiles = "Testfiles\\";

        private static readonly IEnumerable<string> TestFilesEcm = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFiles);

        [Test, TestCaseSource("TestFilesEcm")]
        public void TestEcmCustomConfig(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFiles + testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            ElvizConfiguration[] configurations = test.Setup.ElvizConfigurations;

            if (configurations == null||configurations.Length==0)
            {
                Assert.Fail("Test expected to contain Custom Elviz Configuration");
            }

            ElvizConfigurationTool utility = new ElvizConfigurationTool();
            utility.UpdateConfiguration(configurations);
            EcmTestUtil.TestECM(test,true);

        }


    }

    //[Category("DwhExportOnly")] is not for regression purposes
    //it was used when creating new DWH export
    //can be used for checking calculation
    public class EcmCustomConfigDwhOnly
    {
        private const string TestFiles = "Testfiles\\";

        private static readonly IEnumerable<string> TestFilesEcm = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFiles);

        [Test, TestCaseSource("TestFilesEcm")]
        [Category("DwhExportOnly")]
        public void TestEcmCustomConfig(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFiles + testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            ElvizConfiguration[] configurations = test.Setup.ElvizConfigurations;

            if (configurations == null || configurations.Length == 0)
            {
                Assert.Fail("Test expected to contain Custom Elviz Configuration");
            }

            EcmTestUtil.TestECM(test, false);

        }
    }

        
}
