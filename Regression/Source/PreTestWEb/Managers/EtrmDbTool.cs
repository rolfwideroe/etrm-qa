using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ElvizTestUtils.DatabaseTools;
using PreTestWeb.Models;

namespace PreTestWeb.Managers
{
    class EtrmDbTool
    {
 

        private static SqlConnection GetEcmConnection(Server server)
        {
            SqlServer sqlServer = server.SqlServer;
            return GetSqlConnection(sqlServer.SqlServerName, server.EcmDb, sqlServer.SqlUserName, sqlServer.SqlPassword);
        }

        public static SqlConnection GetDwhConnection(Server server)
        {
            SqlServer sqlServer = server.SqlServer;
            return GetSqlConnection(sqlServer.SqlServerName,server.Dwh, sqlServer.SqlUserName,sqlServer.SqlPassword);
        }

        public static SqlConnection GetSystemConnection(Server server)
        {
            SqlServer sqlServer = server.SqlServer;
            return GetSqlConnection(sqlServer.SqlServerName, server.SystemDb, sqlServer.SqlUserName, sqlServer.SqlPassword);
        }


        private static SqlConnection GetSqlConnection(string sqlInstance,string database,string userName,string password)
        {
            return new SqlConnection(
                $"Data Source={sqlInstance};Initial Catalog={database};User ID={userName};Password={password};Trusted_Connection=False;");
        }

    

        public static void UpdateReportDateForJob (Server server, Workspace workspace,DateTime reportDate)
        {
            SystemDao.TurnOfAutomaticOpening(GetSystemConnection(server));
            JobDao.UpdateReportDateForJob(GetEcmConnection(server), GetSystemConnection(server),
                workspace.WorkspaceName, reportDate,workspace.ErmEcm);
        }

        public static IEnumerable<Tuple<string, string>> GetMonitorsFromWorkspace(PreTest preTest,string workspaceName)
        {

            IList<Tuple<string, string>> typeNamesErm = WorkspaceDao.GetMonitorsInErmWorkspace(workspaceName, GetSystemConnection(preTest.TestServer));
            IList<Tuple<string, string>> typeNamesEcm = WorkspaceDao.GetMonitorsInEcmWorkspace(workspaceName, GetEcmConnection(preTest.TestServer));

            return typeNamesEcm.Union(typeNamesErm);
        }


        public static DataTable GetDataTableFromDwh(Server server, string sqlQuery)
        {
            SqlConnection dwhConnection = GetDwhConnection(server);

            return QaDao.DataTableFromSql(dwhConnection, sqlQuery);


        }

        public static IEnumerable<CommonWorkspace> GetCommonWorkspaces(Server server)
        {
            IEnumerable<string> ecmWorkspaces = WorkspaceDao.GetEcmCommonWorkspaces(GetEcmConnection(server));
            IEnumerable<string> ermWorkspaces = WorkspaceDao.GetErmCommonWorkspaces(GetSystemConnection(server));

            IList<CommonWorkspace> commonWorkspaces = new List<CommonWorkspace>();

            if (ecmWorkspaces != null)
            {
                foreach (string ecm in ecmWorkspaces)
                {
                    commonWorkspaces.Add(new CommonWorkspace()
                    {
                        DisplayName = ecm + " (ECM)",
                        WorkspaceName = ecm,
                        EcmErm = EcmErm.ECM
                    });
                }
            }

            if (ermWorkspaces != null)
            {
                foreach (string erm in ermWorkspaces)
                {
                    commonWorkspaces.Add(new CommonWorkspace()
                    {
                        DisplayName = erm + " (ERM)",
                        WorkspaceName = erm,
                        EcmErm = EcmErm.ERM
                    });
                }
            }
            return commonWorkspaces;
        }

        public static void CreateWorkspaceJob(Workspace workspace,Server server)
        {
            switch (workspace.ErmEcm)
            {
                case "ECM":
                    JobDao.CreateJobFromEcmWorkspaces(GetEcmConnection(server),workspace.WorkspaceName);
                    break;
                case "ERM":
                    JobDao.CreateJobFromErmWorkspaces(GetEcmConnection(server),workspace.WorkspaceName);
                    break;
                default:
                    throw new ArgumentException("Workspace expected to be ECM/ERM but was: "+workspace.ErmEcm);
            }
        }

 


    }
}
