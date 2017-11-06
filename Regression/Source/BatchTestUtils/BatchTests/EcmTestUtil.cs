using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.DatabaseTools;
using NUnit.Framework;

namespace ElvizTestUtils.BatchTests
{
	public class EcmTestUtil
	{

		/// <summary>
		/// Exports specific workspace to the DWH
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="workSpaceName"></param>
		/// <param name="reportDate"></param>
		public static string ExportWorkspaceToDataWareHouse(string userName, string password, string workSpaceName, DateTime reportDate)
		{
			string installationName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
			string d = DateTime.Now.ToString("yyyy-mm-dd_hhmmss");
			string logPath = Path.GetFullPath($@"Logs\BatchLog-RegressionTest-{workSpaceName + "_" + d}.txt");

			if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

			string args =
				$"-batch -batchJobName=\"TestRegEcm\" -jobType=\"RunReport\" -installation=\"{installationName}\" -userName=\"{userName}\" -userPassword=\"{password}\" -workspaceName=\"{workSpaceName}\" -absoluteReportDate=\"{reportDate.ToString("yyyy-MM-dd")}\" -logFileName=\"{logPath}\" -smtpHost=\"\" -emailRecipient=\"\"";

			ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(ElvizInstallationUtility.GetEtrmHome(), "ElvizCm.exe"), args) { UseShellExecute = false };

			Process p = Process.Start(startInfo);
			p.WaitForExit(GlobalConstTestSettings.MAX_BATCH_WAIT_TIME);

			if (p.HasExited == false)
			{
				//Process is still running.
				//Test to see if the process is hung up.
				if (p.Responding)
				{
					//Process was responding; close the main window.
					p.CloseMainWindow();
				}
				else
				{
					//Process was not responding; force the process to close.
					p.Kill();
				}
			}

			return File.Exists(logPath) ? File.ReadAllText(logPath) : "Log file was not created";
		}

		public static void ExportAllTransactionsToDwh()
		{
			bool hasAlreadyBeenExported = QaDao.HasAllTransactionBeenExportedToDwh();

			if (!hasAlreadyBeenExported)
				ExportWorkspaceToDataWareHouse("Vizard", "elviz", "AllTransactions",
					new DateTime(2015, 11, 25));
		}

		public static string AssertLog(string log)
		{
			string[] logLines = log.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			log = "";

			//First 5 lines does not give any useful
			for (int i = 5; i < logLines.Length; i++)
			{
				if (!string.IsNullOrEmpty(logLines[i]))
				{
					string logLine = logLines[i];
					if (!string.IsNullOrEmpty(logLine))
						log = log + logLine + Environment.NewLine;
				}
			}

			bool isMissingTransactions = log.Contains("The selected filters does not return any relevant transactions or Gencon contracts");

			if (isMissingTransactions)
			{
				Assert.Fail("The Workspace is missing Transactions in Filter");
			}

			bool isItemWithTheSameKey = log.Contains("An item with the same key has already been added.");

			if (isItemWithTheSameKey)
			{
				return "RERUN";
			}

			bool hasError = log.ToUpper().Contains("FAILED");

			if (!hasError)
				hasError = log.ToUpper().Contains("ERROR");

			if (hasError)
				Assert.Fail(log);

			return log;
		}

		public static void TestMutablePosMon(BatchTestFile test, bool executeBatch)
		{
			QaDao.ConfigureMutablePosMon(test.Setup);
			TestECM(test, executeBatch);
		}

		public static void TestECM(BatchTestFile test, bool executeBatch)
		{
			if (executeBatch)
			{
				//if MissingRealizedDataStrategy set in file get the original from db, then set the one in file before setting db back to the original
				string origMissingRealizedDataStrategy = string.Empty;
				try
				{
					if (!String.IsNullOrEmpty(test.Setup.MissingRealizedDataStrategy))
					{
						origMissingRealizedDataStrategy = ConfigurationTool.MissingRealizedDataStrategy;
						ConfigurationTool.MissingRealizedDataStrategy = test.Setup.MissingRealizedDataStrategy;
					}


					string log = ExportWorkspaceToDataWareHouse(test.Setup.User, test.Setup.Password, test.Setup.Workspace, test.Setup.ReportDate);

					string assertResult = AssertLog(log);
					if (assertResult == "RERUN")
					{
						Console.WriteLine("Test returned: An item with the same key has already been added. Rerunning one more time." + test.Setup.Workspace);
						log = ExportWorkspaceToDataWareHouse(test.Setup.User, test.Setup.Password, test.Setup.Workspace, test.Setup.ReportDate);
					}

					AssertLog(log);
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
				if (assertion.DbQuery == null)
				{
					throw new ArgumentException("Missing Dwh query");
				}

				DataTable expectedTable = assertion.ExpectedDataTable;

				DataTable actualTable = QaDao.DataTableFromSql(dbType, assertion.DbQuery.PreparedSqlQuery);

				if (test.Setup.Tollerance == null)
					ComplexTypeAssert.AssertDataTables(expectedTable, actualTable);
				else
				{
					ComplexTypeAssert.AssertDataTables(expectedTable, actualTable, test.Setup.Tollerance.Value);
				}
			}
		}
	}
}
