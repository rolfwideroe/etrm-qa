using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.ReportEngineTests;
using NUnit.Framework;

namespace ElvizTestUtils.ReportingDbTimeSeries
{

    public class TestReportingDbUtil
    {


        public static string GetFilterFromBatchTest(BatchTestFile batchTest)
        {
           QaDao dao=new QaDao();
            return dao.GetFilterForEcmWorkspace(batchTest.Setup.Workspace);
        }

        public static int ExecuteJob(string jobAlias, string jobType, DateTime reportDate, ElvizConfiguration[] configurations,int timeOutInSeconds)
        {
            ElvizConfigurationTool configurationTool = new ElvizConfigurationTool();
            if (configurations != null)
            {
                configurationTool.UpdateConfiguration(configurations);  
            }
            else
            {
                configurationTool.RevertAllConfigurationsToDefault();
            }
            if (jobType == null)
            {
                jobType = "Reporting Db Calculated Values Export";
            }
            int jobId = JobAPI.GetJobsIdByDescription(jobAlias, jobType);
            
            Dictionary<string, string> jobParams = new Dictionary<string, string>();
            string reportDateString = reportDate.ToString("yyyyMMdd");

            jobParams.Add("RunForDate", reportDateString);

            if (jobType.Equals("Job Sequence")) jobParams = null;
            int executionId = JobAPI.ExecuteAndAssertJob(jobId, jobParams, timeOutInSeconds);

            return executionId;
        }

        public static DataTable GetActualDataTableFromEcm(string alias, DateTime reportDate,Assertion assertion,int jobExecutionId)
        {
            string binDir = AppDomain.CurrentDomain.BaseDirectory;

            string queryPath = Path.Combine(binDir,"..\\ElvizTestUtils\\ReportingDbQueries\\TimeSeriesFromCashFlowMonitor.xml");
            DbQuery query = TestXmlTool.Deserialize<DbQuery>(queryPath);

            string preparedSqlQuery =
                   query.SqlQuery.Replace("{reportDate}", reportDate.ToString("yyyy-MM-dd"))
                       .Replace("{alias}", alias).Replace("{jobExecutionId}", jobExecutionId.ToString()); 

            Console.WriteLine(preparedSqlQuery);

            return GetActualDataTable(preparedSqlQuery);
        }

        public static DataTable GetLatestExecutionActualDataTableFromEcm(string alias, DateTime reportDate, Assertion assertion,string testFilePath)
        {
            string binDir = AppDomain.CurrentDomain.BaseDirectory;

            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\TimeSeriesFromCashFlowMonitorLatestExecution.xml");
            DbQuery query = TestXmlTool.Deserialize<DbQuery>(queryPath);

            string preparedSqlQuery =
                query.SqlQuery.Replace("{reportDate}", reportDate.ToString("yyyy-MM-dd"))
                    .Replace("{alias}", alias);

            //Console.WriteLine(preparedSqlQuery);

            return GetActualDataTable(preparedSqlQuery);
        }

        private static DataTable GetActualDataTable(string preparedSqlQuery)
        {
            string dbType = "ReportingDatabase";
            DataTable actualTable = QaDao.DataTableFromSql(dbType, preparedSqlQuery);

            return actualTable;
        }

        public static DataTable GetActualTimeSeriesDataTable(string jobName, DateTime reportDate,int jobExecutionId,string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\TimeSeriesFromJob.xml");

            return GetActualDataTable(jobName, reportDate, jobExecutionId, queryPath);
        }

        public static DataTable GetActualFeesTimeSeriesDataTable(string jobName, DateTime reportDate, int jobExecutionId, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\FeesTimeSeriesFromJob.xml");

            return GetActualDataTable(jobName, reportDate, jobExecutionId, queryPath);
        }

        public static DataTable GetActualKeyValuesDataTable(string jobName, DateTime reportDate, int jobExecutionId, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\KeyValuesFromJob.xml");

            return GetActualDataTable(jobName, reportDate, jobExecutionId, queryPath);
        }

        public static DataTable GetActualBcrContractExportDataTable(string jobName, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\BcrContractExportFromJob.xml");

            return GetBcrContractExportActualDataTable(jobName, queryPath);
        }

        private static DataTable GetActualDataTable(string jobName, DateTime reportDate, int jobExecutionId, string queryPath)
        {
            DbQuery query = TestXmlTool.Deserialize<DbQuery>(queryPath);

            string preparedSqlQuery =
                query.SqlQuery.Replace("{reportDate}", reportDate.ToString("yyyy-MM-dd"))
                    .Replace("{alias}", jobName).Replace("{jobExecutionId}", jobExecutionId.ToString());

           // Console.WriteLine(preparedSqlQuery);
       
            string dbType = "ReportingDatabase";
            DataTable actualTable = QaDao.DataTableFromSql(dbType, preparedSqlQuery);
            return actualTable;
        }

        private static DataTable GetBcrContractExportActualDataTable(string jobName, string queryPath)
        {
            DbQuery query = TestXmlTool.Deserialize<DbQuery>(queryPath);
            Console.WriteLine(query.SqlQuery);

            string dbType = "BcrContractDatabase";
            DataTable actualTable = QaDao.DataTableFromSql(dbType, query.SqlQuery);
            return actualTable;
        }

        public static DataTable GetExpectedTimeSeriesDataTable(ReportingDBTestCase testCase,string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\TimeSeriesFromJob.xml");

            return ExpectedTimeSeriesDataTable(testCase, queryPath,"TS");
        }

        public static DataTable GetExpectedFeesTimeSeriesDataTable(ReportingDBTestCase testCase, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\FeesTimeSeriesFromJob.xml");

            return ExpectedTimeSeriesDataTable(testCase, queryPath, "FTS");
        }
        public static DataTable GetExpectedKeyValuesDataTable(ReportingDBTestCase testCase, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\KeyValuesFromJob.xml");

            return ExpectedTimeSeriesDataTable(testCase, queryPath, "KV");
        }

        public static DataTable GetExpectedBcrContractExportDataTable(ReportingDBTestCase testCase, string testFilePath)
        {
            string queryPath = Path.Combine(testFilePath, "..\\..\\..\\..\\ElvizTestUtils\\ReportingDbQueries\\BcrContractExportFromJob.xml");

            return ExpectedTimeSeriesDataTable(testCase, queryPath, "BCR");
        }

        private static DataTable ExpectedTimeSeriesDataTable(ReportingDBTestCase testCase, string queryPath,string tableName)
        {
            DbQuery query = TestXmlTool.Deserialize<DbQuery>(queryPath);

            const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            DataTable expectedTable = DataTableHelper.CreateDataTableFromExpectedRecords(tableName, query.Columns,
                testCase.ExpectedValues.ExpectedDataTable.ExpectedRecord, dateTimeFormat);

            return expectedTable;
        }
    }
}
