using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElvizTestUtils;
using ElvizTestUtils.BatchTests;
using NUnit.Framework;

namespace TestEcmBatch
{
    public class TestEcmDwhExportOnly
    {
        //[Category("DwhExportOnly")] is not for regression purposes
        //it was used when creating new DWH export
        //can be used for checking calculation

        private const string TestFiles = "Testfiles\\";
        private static readonly IEnumerable<string> TestFilesECM = TestCasesFileEnumeratorByFolder.TestCaseFiles(TestFiles);

        [Test, TestCaseSource("TestFilesECM")]
        [Category("DwhExportOnly")]
        public void TestECM(string testFile)
        {
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestFiles+ testFile);

            BatchTestFile test = BatchTestFileParser.DeserializeXml(testFilePath);

            EcmTestUtil.TestECM(test, false);

        }

      
    }
}