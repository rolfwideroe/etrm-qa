using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.DatabaseTools;
using NUnit.Framework;

namespace ElvizTestUtils.BatchTests
{
    public class ErmTestUtil
    {
        /// <summary>
        /// Exports specific workspace to the DWH
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="workSpaceName"></param>
        /// <param name="reportDate"></param>
        /// <param name="logPrefix"></param>
        /// <param name="newVersion"></param>
        /// <param name="runInParallel"></param>
        /// <param name="resolution"></param>
        public static string ExportWorkspaceToDataWareHouse(string userName, string password, string workSpaceName, DateTime reportDate, string logPrefix, bool newVersion, bool runInParallel, string resolution)
        {
            string logPath = Path.GetFullPath(string.Format(@"Logs\{0}-BatchLog-RegressionTest-{1}.txt", logPrefix, workSpaceName));

            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

            if (File.Exists(logPath)) File.Delete(logPath);

            string args =
                string.Format(
                    "-batch -batchJobName=\"TestRegErm\" -jobType=\"RunReport\" -installation=\"{0}\" -userName=\"{1}\" -userPassword=\"{2}\" -workspaceName=\"{3}\" -absoluteReportDate=\"{4}\" -logFileName=\"{5}\" -smtpHost=\"\" -emailRecipient=\"\" -runInParallel=\"{6}\" -resolution=\"{7}\"",
                    ElvizInstallationUtility.GetEtrmDbName("VizECM"), userName, password, workSpaceName, reportDate.ToString("yyyy-MM-dd"), logPath, runInParallel, resolution);

            ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(ElvizInstallationUtility.GetEtrmHome(),"ElvizRM.exe"), args) { UseShellExecute = false };
            if (newVersion)
                startInfo = new ProcessStartInfo(Path.Combine(ElvizInstallationUtility.GetEtrmHome(),"DwhExport.exe"), args) { UseShellExecute = false };

            Process p = Process.Start(startInfo);
            p.WaitForExit(GlobalConstTestSettings.MAX_BATCH_WAIT_TIME);

            return File.Exists(logPath) ? File.ReadAllText(logPath) : "Log file was not created";
        }

        public static void DoTestERM(BatchTestFile test, string logPrefix, bool newVersion_calculations,
            bool newVersion_verification, bool runInParallel, string resolution,bool executeBatch)
        {
            string log = "";
            ElvizConfiguration[] elvizConfigurations = test.Setup.ElvizConfigurations;
            //if MissingRealizedDataStrategy set in file get the original from db, then set the one in file before setting db back to the original
            string origMissingRealizedDataStrategy = string.Empty;


            if (executeBatch)
            {

                try
                {



                    if (!String.IsNullOrEmpty(test.Setup.MissingRealizedDataStrategy))
                    {
                        origMissingRealizedDataStrategy =
                            ConfigurationTool.MissingRealizedDataStrategy;
                        ConfigurationTool.MissingRealizedDataStrategy =
                            test.Setup.MissingRealizedDataStrategy;
                    }

                    DateTime reportDate = test.Setup.ReportDate;

                    log = ExportWorkspaceToDataWareHouse(test.Setup.User, test.Setup.Password, test.Setup.Workspace,
                        reportDate, logPrefix, newVersion_calculations, runInParallel,
                        resolution);
                    VerifyLog(log,newVersion_verification);
                }
                finally
                {
                    //if set in file set back to original
                    if (!String.IsNullOrEmpty(test.Setup.MissingRealizedDataStrategy))
                        ConfigurationTool.MissingRealizedDataStrategy = origMissingRealizedDataStrategy;
                }
            }



            string dbType = "VizDatawarehouse";
            foreach (Assertion assertion in test.Assertions)
            {
                if (assertion.DbQuery == null) throw new ArgumentException("Missing DWH query");
                

                DataTable expectedTable = assertion.ExpectedDataTable;
                DataTable actualTable =
                    QaDao.DataTableFromSql(dbType, assertion.DbQuery.PreparedSqlQuery);

                ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
            }

        }

        public static void VerifyLog(string log,bool newVersionVerification)
        {
            string newErrorMessage = "";
            string[] logLines = log.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            log = "";

            //First 8 lines does not give any usefull
            int start = 8;
            if (newVersionVerification)
                start = 0;
            for (int i = start; i < logLines.Length; i++)
            {
                string logLine = logLines[i];

         

                if (!string.IsNullOrEmpty(logLine))
                {
                    

                        string firstTwoChar = new string(logLine.ToCharArray(0, 2));

                    int firstTwoInt;

                    if (!logLine.Contains("[1] INFO"))
                        newErrorMessage += logLine + Environment.NewLine;

                    string formattedLogLine;
                    if (int.TryParse(firstTwoChar, out firstTwoInt))
                    {
                        formattedLogLine = logLines[i].Substring(20);
                        if (!string.IsNullOrEmpty(formattedLogLine))
                            log = log + formattedLogLine + Environment.NewLine;

                        
                    }
                    else
                    {
                        log = log + logLine + Environment.NewLine;
                    }
                }
            }

            bool isMissingTransactions =
                log.Contains(
                    "The selected filters does not return any relevant transactions or Gencon contracts");

            if (isMissingTransactions)
            {
                Assert.Fail("The Workspace is missing Transactions in Filter");
            }

            bool hasError = log.ToUpper().Contains("FAILED") || log.ToUpper().Contains("ERROR");

            if (hasError)
            {
                if(newVersionVerification)
                    Assert.Fail(newErrorMessage);

                Assert.Fail(log);
            }
                
        }
    }
}
