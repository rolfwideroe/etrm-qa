using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElvizTestUtils.DatabaseTools
{
    public class WorkspaceDao
    {


        public static IEnumerable<string> GetErmCommonWorkspaces(SqlConnection systemConnection)
        {


            const string query = @"
                                    SELECT p.EP_PropertyValue as Workspace FROM EP_ElementProperties p

                                    JOIN ED_ElementData d on p.EP_ED_Id=d.ED_Id
                                    JOIN EC_ElementCategory c on c.EC_Id=d.ED_EC_Id

                                    WHERE P.EP_ED_Id IN (
                                    (SELECT p.EP_ED_Id FROM EP_ElementProperties p
                                    WHERE p.EP_ED_Id IN (
                                    SELECT p.EP_ED_Id FROM EP_ElementProperties p
                                    WHERE p.EP_PropertyName='workspacetype'
                                    AND p.EP_PropertyValue='Common')
                                    AND p.EP_PropertyName='workspacelocked'
                                    AND p.EP_PropertyValue='True'))

                                    AND p.EP_PropertyName='name'
                                    AND d.ED_AppName='ERM'
                                    AND c.EC_Name='workspace'
                                    order by p.EP_PropertyValue";

           DataTable table=QaDao.DataTableFromSql(systemConnection, query);

            if (table.Rows.Count == 0) return null;

            IList<string> workspace = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                workspace.Add(dataRow["Workspace"] as string);
            }


            return workspace;
        }

        public static IEnumerable<string> GetEcmCommonWorkspaces(SqlConnection ecmConnection)
        {


            const string query = @"
                                    SELECT WSName as Workspace FROM Workspace
                                    WHERE ShareType='Common'
                                    AND Locked=1";

            DataTable table = QaDao.DataTableFromSql(ecmConnection, query);

            if (table.Rows.Count == 0) return null;

            IList<string> workspace = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                workspace.Add(dataRow["Workspace"] as string);
            }


            return workspace;
        }

        public static void CreateJobFromWorkspace(string workspace, string ermOrEcm,SqlConnection ecmConnection)
        {
            string query = @"	if not exists (select * from StoredJobs WHERE [Description]=@wsName)  

		                        INSERT INTO StoredJobs VALUES('Run Workspace Job',wsName)
                                BEGIN
		                        declare @maxStoredJobId INT
		                        set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

		                        INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL)

		                        INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceName',@wsName)
		                        INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceType',@ermOrEcm)
		                        INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportDateOffset','0')
		                        INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Username','Vizard')
		                        INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Password','elviz')
                                END";

            IList<SqlParameter> parameters=new List<SqlParameter>();
            parameters.Add(new SqlParameter("@wsName",workspace));
            parameters.Add(new SqlParameter("@ermOrEcm", ermOrEcm));


            QaDao.NonQuery(ecmConnection, query, parameters);

        }

        public static IList<Tuple<string, string>> GetMonitorsInEcmWorkspace(string workspaceName, SqlConnection ecmConnection)
        {
            IList<Tuple<string, string>> monitorTypeMonitorName = new List<Tuple<string, string>>();

            const string query = @"SELECT f.FormName as MonitorType ,s.Value as MonitorName FROM Workspace w
                            INNER JOIN WSForm f on f.WSId=w.WSId
                            INNER JOIN WSFormSettings s on s.FormId=f.FormId and s.WSId=w.WSId
                            WHERE w.ShareType='Common'
                            AND s.Setting='CustomName'
                            and w.WSName=@workspaceName";

            SqlCommand command = new SqlCommand(query, ecmConnection);
            command.Parameters.Add("@workspaceName", SqlDbType.VarChar, 80).Value = workspaceName;

            try
            {
                ecmConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string monitorType = reader.GetString(0);
                    string monitorName = reader.GetString(1);

                    monitorTypeMonitorName.Add(new Tuple<string, string>(monitorType, monitorName));
                }

            }
            finally
            {
                ecmConnection.Close();
            }
            return monitorTypeMonitorName;
        }

        public static IList<Tuple<string, string>> GetMonitorsInErmWorkspace(string workspaceName, SqlConnection systemConnection)
        {
            

            const string query = @"
                                  
                                declare @s NVARCHAR(MAX)
                                SET @s=(
                                SELECT d.ED_Data FROM EP_ElementProperties p

                                JOIN ED_ElementData d on p.EP_ED_Id=d.ED_Id
                                JOIN EC_ElementCategory c on c.EC_Id=d.ED_EC_Id

                                WHERE P.EP_ED_Id IN (
                                (SELECT p.EP_ED_Id FROM EP_ElementProperties p
                                WHERE p.EP_ED_Id IN (
                                SELECT p.EP_ED_Id FROM EP_ElementProperties p
                                WHERE p.EP_PropertyName='workspacetype'
                                AND p.EP_PropertyValue='Common')
                                AND p.EP_PropertyName='workspacelocked'
                                AND p.EP_PropertyValue='True'))

                                AND p.EP_PropertyName='name'
                                AND d.ED_AppName='ERM'
                                AND c.EC_Name='workspace'
                                and p.EP_PropertyValue=@workspaceName
                                )
							   
                                declare @input NVARCHAR(MAX)
                                SET @input=(SELECT REPLACE(@s,'""',''))					
                                DECLARE @str VARCHAR(MAX)

                                DECLARE @ind Int
                                DECLARE @Result TABLE(v varchar(4000))
                                IF(@input is not null)

                                BEGIN
                                    SET @ind = CharIndex(',', @input)
                                    WHILE @ind > 0
                                    BEGIN
                                            SET @str = SUBSTRING(@input, 1, @ind - 1)
                                            SET @input = SUBSTRING(@input, @ind + 1, LEN(@input) - @ind)
                                            INSERT INTO @Result values (@str)
                                            SET @ind = CharIndex(',', @input)
                                    END
                                    SET @str = @input
                                    INSERT INTO @Result values(@str)
                                END

                                select REPLACE(v, 'ReportCaption=', '') as Monitor from @Result
                                  WHERE v like 'ReportCaption=%' or v like '%ReportType=%'

                                ";

            SqlCommand command = new SqlCommand(query, systemConnection);
            command.Parameters.Add("@workspaceName", SqlDbType.VarChar, 80).Value = workspaceName;

            DataTable table=new DataTable();

            try
            {
                systemConnection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(table);

            }
            finally
            {
                systemConnection.Close();
            }

            IList<Tuple<string, string>> monitorTypeMonitorName = new List<Tuple<string, string>>();
            int i = 0;
            while (i<table.Rows.Count)
            {
                string monitorTypeUnformated = table.Rows[i]["Monitor"] as string;
                string monitor = table.Rows[i+1]["Monitor"] as string;
                string monitorType = monitorTypeUnformated.Split('=').Last();
                monitorTypeMonitorName.Add(new Tuple<string, string>(monitorType, monitor));
                i += 2;
            }

            return monitorTypeMonitorName;
        }
    }
}
