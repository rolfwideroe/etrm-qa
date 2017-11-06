using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElvizTestUtils.BatchTests;

namespace ElvizTestUtils.DatabaseTools
{
    public class JobDao
    {
        const string CreateJobFromWorkspace = @"if not exists (select * from StoredJobs j
				                                                join ScheduledJobs s on s.StoredJobId=j.StoredJobId
				                                                WHERE [Description]=@jobName)  
	                                                begin

		                                                INSERT INTO StoredJobs VALUES('Run Workspace Job',@jobName)

		                                                declare @maxStoredJobId INT
		                                                set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

		                                                INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL)

		                                                INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceName',@jobName)
		                                                INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceType',@ecmErm)
		                                                INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportDateOffset','0')
		                                                INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Username','Vizard')
		                                                INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Password','elviz')

	                                                end";

        public static void UpdateReportDateForJob(SqlConnection ecmConnection,SqlConnection systemConnection, string workspace, DateTime reportDate,string ermEcm)
        {


            SystemDao.UpdateSystemDateToToday(systemConnection);
    
            TimeSpan span = DateTime.Today - reportDate;

            int offSet = span.Days;

            string ecmQuery = @" update  p set p.Value=" + offSet + @"
                                FROM StoredJobParameters p
                                inner join ScheduledJobs sch on sch.StoredJobId = p.StoredJobId
                                inner join StoredJobs sto on sto.StoredJobId = sch.StoredJobId
                                WHERE sto.JobTypeName = 'Run Workspace Job'
                                AND sto.Description = '" + workspace + @"'
                                AND p.ParameterName = 'ReportDateOffset'";

            QaDao.NonQuery(ecmConnection, ecmQuery);
            Thread.Sleep(1000);
        }

        public static void CreateJobFromEcmWorkspaces(SqlConnection ecmConnection,string workspaceName)
        {
            IList<SqlParameter> parameters=new List<SqlParameter>() ;
            parameters.Add(new SqlParameter("@jobName",workspaceName));
            parameters.Add(new SqlParameter("@ecmErm", "ECM"));

            QaDao.NonQuery(ecmConnection,CreateJobFromWorkspace,parameters);
        }

        public static void CreateJobFromErmWorkspaces(SqlConnection ecmConnection, string workspaceName)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@jobName", workspaceName));
            parameters.Add(new SqlParameter("@ecmErm", "ERM"));

            QaDao.NonQuery(ecmConnection, CreateJobFromWorkspace, parameters);
        }
    }
}
