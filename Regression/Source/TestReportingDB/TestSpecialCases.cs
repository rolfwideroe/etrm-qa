using System;
using System.Collections.Generic;
using System.IO;
using ElvizTestUtils;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.ReportingDbTimeSeries;
using NUnit.Framework;
using ExpectedValues = ElvizTestUtils.ReportingDbTimeSeries.ExpectedValues;
using InputData = ElvizTestUtils.ReportingDbTimeSeries.InputData;


namespace TestReportingDB
{

    [TestFixture]
    public class TestSpecialCases
    {


        [TestFixtureSetUp]
        public void Setup()
        {
            ReportDbTestsSetup.LoadStaticData();

        }

        private const string SpecialCasesFolder = "TestFiles\\SpecialCases";

        private static readonly IEnumerable<string> SpecialCases = TestCasesFileEnumeratorByFolder.TestCaseFiles(SpecialCasesFolder);


        [Test, TestCaseSource(nameof(SpecialCases)), Timeout(300 * 1000)]
        public void Test(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), SpecialCasesFolder, fileName);
            ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);
            ReportDbTestUtil.TestTimeSeries(testCase, filePath);
        }




    }
}
