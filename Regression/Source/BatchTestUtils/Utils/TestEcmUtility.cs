using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace EcmBatchTests.Utils
{
    public class TestEcmUtility
    {
        private const int MAX_WAIT_TIME = 100000;

        private static string _dataWareHouseDatabaseName = "";
        private static string _sqlServerName = "";
        private static string _databaseUsername = "";
        private static string _databasePassword = "";
        private static string _installationName = "";

        static TestEcmUtility()
        {
            _sqlServerName = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\Servers", "Default");

            _installationName = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + _sqlServerName, "VizEcm");

            _dataWareHouseDatabaseName = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + _sqlServerName, "VizDatawarehouse");

            _databaseUsername = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + _sqlServerName, "DefaultUser");

            _databasePassword = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\" + _sqlServerName, "DefaultPw");
        }

        static string GetRegistryValue(string path, string valueName)
        {
            var res = Registry.GetValue(path, valueName, "").ToString();

            if (string.IsNullOrEmpty(res))
                throw new ApplicationException(string.Format(@"Cannot read sql server name from {0}->{1}", path, valueName));

            return res;
        }

        static string GetElvizFolder()
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo", "InstallPath", @"C:\Elviz").ToString();
        }        

        private static string GetCurrentInstallationName()        
        {
            return _installationName;
        }

        /// <summary>
        /// Exports specific workspace to the DWH
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="workSpaceName"></param>
        /// <param name="reportDate"></param>
        public static string ExportWorkspaceToDataWareHouse(string userName, string password, string workSpaceName, string reportDate)
        {
            var installationName = GetCurrentInstallationName();
            string logPath = Path.GetFullPath(string.Format(@"Logs\BatchLog-RegressionTest-{0}.txt", workSpaceName));

            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

            if (File.Exists(logPath)) File.Delete(logPath);

            var args =
                string.Format(
                    "-batch -batchJobName=\"TestRegEcm\" -jobType=\"RunReport\" -installation=\"{0}\" -userName=\"{1}\" -userPassword=\"{2}\" -workspaceName=\"{3}\" -absoluteReportDate =\"{4}\" -logFileName=\"{5}\" -smtpHost=\"\" -emailRecipient=\"\"",
                    installationName, userName, password, workSpaceName, reportDate, logPath);

            var startInfo = new ProcessStartInfo(Path.Combine(GetElvizFolder(), "Bin", "ElvizCm.exe"), args) { UseShellExecute = false };

            var p = Process.Start(startInfo);
            p.WaitForExit(MAX_WAIT_TIME);

            return File.ReadAllText(logPath);
        }

        public static object[] ExecuteSqlAgainstDataWareHouse(string sql)
        {
            if (string.IsNullOrEmpty(_dataWareHouseDatabaseName)) throw new ApplicationException("Datawarehouse Property has not been initialized");

            var connection = new SqlConnection(string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Trusted_Connection=False;", _sqlServerName, _dataWareHouseDatabaseName, _databaseUsername, _databasePassword));

            var res = new List<object[]>();

            var command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var values = new Object[reader.FieldCount];
                    reader.GetValues(values);
                    res.Add(values);
                }
            }
            finally
            {
                connection.Close();
            }

            return res.ToArray();
        }
    }
}
