using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using NUnit.Framework;

namespace TestWCFDealInsertWithResultMessage
{
    [TestFixture]
    public class TestWCFInsertAuthorisationsOn
    {
        private const string TestFilesAuthPath = "TestFilesAuthorisations\\";
        private static readonly IEnumerable<string> TestFilesAuthorisations = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFilesAuthPath);
        [TestFixtureSetUp]
        public void Setup()
        {
            if (!ConfigurationTool.AutorizationEnabled)
            {
                ConfigurationTool.AutorizationEnabled = true;
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if (ConfigurationTool.AutorizationEnabled)
            {
                ConfigurationTool.AutorizationEnabled = false;
            }
        }

        [Test, Timeout(2000 * 1000), TestCaseSource("TestFilesAuthorisations")]
        public void InsertDealWithAutorizationOn(string testFile)
        {
           
            //Console.WriteLine(ConfigurationTool.AutorizationEnabled);
            
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFilesAuthPath + testFile);

            TestWcfDealInsertWithResultMessage.TestWcfDealInsertByTestFile(testFilePath);
            

        }
    }
}
