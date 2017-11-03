using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.ReportingDbTimeSeries;

namespace TestReportingDB
{
    public class ReportDbTestUtil
    {
        public static void TestTimeSeries(ReportingDBTestCase testCase, string testFilePath)
        {
            int executionId = ExecuteReportingDbJob(testCase);

            DataTable expectedTable = TestReportingDbUtil.GetExpectedTimeSeriesDataTable(testCase, testFilePath);
            DataTable actualTable = TestReportingDbUtil.GetActualTimeSeriesDataTable(testCase.InputData.JobName,
                testCase.InputData.ReportDate, executionId, testFilePath);

            ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
        }

        public static void TestFeesTimeSeries(ReportingDBTestCase testCase, string testFilePath)
        {
            int executionId = ExecuteReportingDbJob(testCase);

            DataTable expectedTable = TestReportingDbUtil.GetExpectedFeesTimeSeriesDataTable(testCase, testFilePath);
            DataTable actualTable = TestReportingDbUtil.GetActualFeesTimeSeriesDataTable(testCase.InputData.JobName,
                testCase.InputData.ReportDate, executionId, testFilePath);

            ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
        }

        public static void TestKeyValues(ReportingDBTestCase testCase, string testFilePath)
        {
            int executionId = ExecuteReportingDbJob(testCase);

            DataTable expectedTable = TestReportingDbUtil.GetExpectedKeyValuesDataTable(testCase, testFilePath);
            DataTable actualTable = TestReportingDbUtil.GetActualKeyValuesDataTable(testCase.InputData.JobName,
                testCase.InputData.ReportDate, executionId, testFilePath);

            ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
        }

        public static void TestBcrContractExport(ReportingDBTestCase testCase, string testFilePath)
        {
            ExecuteReportingDbJob(testCase);

            DataTable expectedTable = TestReportingDbUtil.GetExpectedBcrContractExportDataTable(testCase, testFilePath);
            DataTable actualTable = TestReportingDbUtil.GetActualBcrContractExportDataTable(testCase.InputData.JobName,testFilePath);

            ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
        }

        private static int ExecuteReportingDbJob(ReportingDBTestCase testCase)
        {
            string jobName = testCase.InputData.JobName;
            string jobType = testCase.InputData.JobType;
            DateTime reportDate = testCase.InputData.ReportDate;

            int timeOut = 300;

            if (testCase.InputData.TimeOutInSeconds != null) timeOut = (int) testCase.InputData.TimeOutInSeconds;

            string origMissingRealizedDataStrategy = string.Empty;
            int executionId = 0;
            try
            {
                if (!string.IsNullOrEmpty(testCase.InputData.MissingRealizedDataStrategy))
                {
                    origMissingRealizedDataStrategy = ConfigurationTool.MissingRealizedDataStrategy;
                    ConfigurationTool.MissingRealizedDataStrategy = testCase.InputData.MissingRealizedDataStrategy;
                }

                executionId = TestReportingDbUtil.ExecuteJob(jobName,jobType, reportDate, testCase.InputData.ElvizConfigurations,timeOut);
            }
            finally
            {
                //if set in file set back to original
                if (!string.IsNullOrEmpty(testCase.InputData.MissingRealizedDataStrategy))
                    ConfigurationTool.MissingRealizedDataStrategy = origMissingRealizedDataStrategy;
            }
            return executionId;
        }


    
    }
}
