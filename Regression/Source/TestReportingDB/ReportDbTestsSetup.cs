using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;

namespace TestReportingDB
{
    class ReportDbTestsSetup
    {
        private static readonly IEnumerable<string> ScriptFiles = TestCasesFileEnumeratorByFolder.TestCaseFiles(ScriptFolder);

        private const string ScriptFolder = "JobScripts\\RunInSetup\\";
        public static void LoadStaticData()
        {
            try
            {
                ElvizTestUtils.LoadStaticData.LoadStaticData loadStatic = new ElvizTestUtils.LoadStaticData.LoadStaticData();
                loadStatic.InsertPortfolios();
                loadStatic.RunEcmDbScripts();
                ExecuteScripts();
                ElvizConfigurationTool configurationTool = new ElvizConfigurationTool();
                configurationTool.RevertAllConfigurationsToDefault();

                //Sync contracts
               // JobAPI.ExecuteAndAssertJob(1, 300);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                throw;
            }
        }

        private static void ExecuteScripts()
        {
            string ecmDbName = ElvizInstallationUtility.GetEtrmDbName("VizECM");
            string sqlServerName = ElvizInstallationUtility.GetSqlServerName();
            SqlConnection connection = new SqlConnection($"Data Source={sqlServerName};Initial Catalog={ecmDbName};Trusted_Connection=True;");

            foreach (string scriptFile in ScriptFiles)
            {
                Console.WriteLine("Executing script:" + scriptFile);
                string scriptPath = Path.Combine(ScriptFolder, scriptFile);

                string script = File.ReadAllText(scriptPath);

                QaDao.NonQuery(connection, script);
            }

            Thread.Sleep(7000);
        }
    }

}
