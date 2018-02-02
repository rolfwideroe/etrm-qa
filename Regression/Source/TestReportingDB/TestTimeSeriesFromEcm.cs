using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using ElvizTestUtils;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.QaLookUp;
using ElvizTestUtils.ReportingDbTimeSeries;
using NUnit.Framework;

namespace TestReportingDB
{
    //[TestFixture]
    public class TestTimeSeriesFromEcm
    {
        private QaDao qaDao;
       
        [SetUp,Timeout(610*1000)]
        public void Setup()
        {
            this.qaDao=new QaDao();
            //Sync contracts
              JobAPI.ExecuteAndAssertJob(1, 600);

           ExecuteScripts();

            
        }

        private const string ScriptFolder = "JobScripts\\RunInSetupFromECM\\";
        private static readonly IEnumerable<string> ScriptFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles(ScriptFolder);
        private void ExecuteScripts()
        {
            string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            SqlConnection connection = new SqlConnection($"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");

            foreach (string scriptFile in ScriptFiles)
            {
                string scriptPath = Path.Combine(ScriptFolder, scriptFile);

                string script = File.ReadAllText(scriptPath);

                QaDao.NonQuery(connection, script);
            }
        }

        //[NUnit.Framework.SetUp]
        //public void Runfirst()
        //{
        //    throw this.ex;
        //}




        private const string PlFromEcmFolder = "TestFiles\\PLFromEcm\\";
        private const string MWhFromEcmFolder = "TestFiles\\MWhFromEcm\\";
        private const string VolumeFromEcmFolder = "TestFiles\\VolumeFromEcm\\";
        private const string CFFromEcmFolder = "TestFiles\\CFFromEcm\\";
		private const string PlNOKFromEcmFolder = "TestFiles\\PLNOKFromEcm\\";

		private static readonly IEnumerable<string> TestFilesEcmFolderMWh =TestCasesFileEnumeratorByFolder.TestCaseFiles(MWhFromEcmFolder);
        private static readonly IEnumerable<string> TestFilesEcmFolderVolume =TestCasesFileEnumeratorByFolder.TestCaseFiles(VolumeFromEcmFolder);

        private static readonly IEnumerable<string> TestFilesEcmFolderPl =TestCasesFileEnumeratorByFolder.TestCaseFiles(PlFromEcmFolder);
        private static readonly IEnumerable<string> TestFilesEcmFolderCF =TestCasesFileEnumeratorByFolder.TestCaseFiles(CFFromEcmFolder);
		private static readonly IEnumerable<string> TestFilesEcmFolderPLNOK = TestCasesFileEnumeratorByFolder.TestCaseFiles(PlNOKFromEcmFolder);


		//[Test, TestCaseSource("TestFilesEcmFolderPLNOK"), Timeout(60*1000)]
		public void TestPLNOKFromEcm(string testFilePath)
        {
            BatchTestFile test = BatchTestFileParser.DeserializeXml(PlNOKFromEcmFolder+testFilePath);

            Test(test,null);
        }

		//  [Test, TestCaseSource("TestFilesEcmFolderPl"), Timeout(60*1000)]
		public void TestPLFromEcm(string testFilePath)
		{
			BatchTestFile test = BatchTestFileParser.DeserializeXml(PlFromEcmFolder + testFilePath);

			Test(test, null);
		}


		//     [Test, TestCaseSource("TestFilesEcmFolderMWh"), Timeout(60 * 1000)]
		public void TestMWhFromEcm(string testFilePath)
        {
            BatchTestFile test = BatchTestFileParser.DeserializeXml(MWhFromEcmFolder + testFilePath);

            Test(test,new string[] {"Currency"});
        }

//        [Test, TestCaseSource("TestFilesEcmFolderVolume"), Timeout(60 * 1000)]
        public void TestVolOrgFromEcm(string testFilePath)
        {
            BatchTestFile test = BatchTestFileParser.DeserializeXml(VolumeFromEcmFolder + testFilePath);


            Test(test, new string[] { "Currency" });
        }

//        [Test, TestCaseSource("TestFilesEcmFolderCF"), Timeout(60 * 1000)]
        public void TestCFFromEcm(string testFilePath)
        {
            BatchTestFile test = BatchTestFileParser.DeserializeXml(CFFromEcmFolder + testFilePath);


            Test(test, new string[] { "Currency" });
        }


        private void Test(BatchTestFile test,string[] ignoreColmns)
        {
            //string filter = TestReportingDbUtil.GetFilterFromBatchTest(test);
            //string jobAlias = testFilePath.Replace(".xml", "");

            DateTime reportDate = test.Setup.ReportDate;

            string origMissingRealizedDataStrategy = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(test.Setup.MissingRealizedDataStrategy))
                {
                    origMissingRealizedDataStrategy = ConfigurationTool.MissingRealizedDataStrategy;
                    ConfigurationTool.MissingRealizedDataStrategy = test.Setup.MissingRealizedDataStrategy;
                }

                string jobAlias = test.Setup.Workspace;
                
                int executionId = TestReportingDbUtil.ExecuteJob(jobAlias, null, reportDate,test.Setup.ElvizConfigurations,45);

                AssertFromEcm(jobAlias, test.Setup.ReportDate, executionId, test.Assertions[0], ignoreColmns);

            }
            finally
            {                 //if set in file set back to original
                if (!String.IsNullOrEmpty(test.Setup.MissingRealizedDataStrategy))
                    ConfigurationTool.MissingRealizedDataStrategy = origMissingRealizedDataStrategy;
            }
           
        }


        private void AssertFromEcm(string alias,DateTime reportDate,int jobExecutionId,  Assertion assertion,string[] ignoreColumns)
        {
            
           
                DataTable expectedTable = assertion.ExpectedDataTable;
                DataTable actualTable = TestReportingDbUtil.GetActualDataTableFromEcm(alias, reportDate, assertion,jobExecutionId);
                
                ComplexTypeAssert.AssertDataTables(expectedTable,actualTable,ignoreColumns);

            
        }
    }
}
