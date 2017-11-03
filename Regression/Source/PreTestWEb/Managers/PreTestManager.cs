using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using ElvizTestUtils;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.Excel;
using ElvizTestUtils.InternalJobService;
using ElvizTestUtils.QaLookUp;
using PreTestWeb.Models;
using DataTable = System.Data.DataTable;
using ClosedXML.Excel;

namespace PreTestWeb.Managers
{
    public class PreTestManager:IDisposable
    {
        private readonly PretestModelEntities dataModel;

        public PreTestManager()
        {
            this.dataModel = new PretestModelEntities();
        }

        private Workspace GetWorkspace(int workspaceId)
        {
            Workspace workspace = this.dataModel.Workspaces.SingleOrDefault(x => x.WorkspaceId == workspaceId);

            if (workspace == null) throw new ArgumentException("Cannot find workspace with id=" + workspaceId);

            return workspace;
        }

        public IEnumerable<CommonWorkspace> GetCommonWorkspaces(int preTestId)
        {
            PreTest preTest = this.dataModel.PreTests.Single(x => x.PreTestId == preTestId);
            return EtrmDbTool.GetCommonWorkspaces(preTest.TestServer);
        }

        public int CreateWorkspace(int preTestId, CommonWorkspace commonWorkspace, DateTime reportDate, int timeOutInSeconds)
        {
            PreTest preTest = this.dataModel.PreTests.Single(x => x.PreTestId == preTestId);

            if (this.dataModel.Workspaces.Count(x => x.WorkspaceName == commonWorkspace.WorkspaceName && x.PreTestId == preTest.PreTestId && x.ReportDate == reportDate) > 0)
            {
                throw new ArgumentException("The Workspace: " + commonWorkspace.WorkspaceName + " with Reportdate: " + reportDate.ToString("dd/MM/yyyy") + " already exists for PreTest: " + preTest.Name);
            }

            IEnumerable<Tuple<string, string>> etrmMonitors = EtrmDbTool.GetMonitorsFromWorkspace(preTest, commonWorkspace.WorkspaceName);

            if (etrmMonitors == null) throw new ArgumentException("Common Workspace: " + commonWorkspace.WorkspaceName + " does not exist or contains no monitors");

            IList<Monitor> monitors = new List<Monitor>();

            foreach (Tuple<string, string> etrmMonitor in etrmMonitors)
            {
                string etrmMonitorType = etrmMonitor.Item1;
                string monitorName = etrmMonitor.Item2;
                MonitorType monitorType = this.dataModel.MonitorTypes.SingleOrDefault(x => x.EtrmName == etrmMonitorType);

                if (monitorType == null) throw new ArgumentException("Monitor type: " + etrmMonitorType + " is unknown, please update MonitorTypes table");

                Monitor monitor = new Monitor
                {
                    MonitorName = monitorName,
                    Query = monitorType.DefaultQuery,
                    MonitorType = monitorType,
                    Tollerance = 0.00000001



                };

                monitors.Add(monitor);
            }

            Workspace workspace = new Workspace
            {
                WorkspaceName = commonWorkspace.WorkspaceName,
                ReportDate = reportDate,
                TimeOutInSeconds = timeOutInSeconds,
                ErmEcm = commonWorkspace.EcmErm.ToString(),
                Monitors = monitors,
                IsRunningInProdEnv = false,
                IsRunningInTestEnv = false
            };

            preTest.Workspaces.Add(workspace);

            this.ValidateAndSaveDataModel();

            return workspace.WorkspaceId;
        }


        private JobExecutionStatus ExceucteJob(Server server, Workspace workspace)
        {
            string workspaceName = workspace.WorkspaceName;
            string serverName = server.ServerName;
            DateTime reportDate = workspace.ReportDate;
            int timeOutInSeconds = workspace.TimeOutInSeconds;

            Job job = JobAPI.FindJob(workspaceName, "Run Workspace Job", serverName);

            if (job == null)
            {
                EtrmDbTool.CreateWorkspaceJob(workspace, server);
                job = JobAPI.FindJob(workspaceName, "Run Workspace Job", serverName);
            }


            int jobId = job.Id;

            EtrmDbTool.UpdateReportDateForJob(server, workspace, reportDate);

            return JobAPI.ExecuteJob(jobId, null, timeOutInSeconds, serverName);
        }

        private void ValidateAndSaveDataModel()
        {

            IList<DbEntityValidationResult> r = this.dataModel.GetValidationErrors().ToList();

            foreach (DbEntityValidationResult dbEntityValidationResult in r)
            {
                foreach (DbValidationError dbValidationError in dbEntityValidationResult.ValidationErrors)
                {
                    Console.WriteLine(dbValidationError.ErrorMessage);
                }
            }

            this.dataModel.SaveChanges();
        }

        public void RunPreTestInTestEnv(int preTestId)
        {
            IEnumerable<Workspace> workspaces = this.dataModel.PreTests.Single(x => x.PreTestId == preTestId).Workspaces;

            foreach (Workspace workspace in workspaces)
            {
                this.RunWorkspaceInTestEnvAndCompare(workspace.WorkspaceId);
            }

        }

        public static DataTable GetDataTableFromDwh(Monitor monitor,bool testServer)
        {
            string query = monitor.Query;

            Server server = testServer ? monitor.Workspace.PreTest.TestServer : monitor.Workspace.PreTest.ProdServer;
           

            string dwhInstallation = SystemDao.GetEtrmInstallation(EtrmDbTool.GetSystemConnection(server));

            string preparedSqlQuery = query.Replace("{reportdate}",
                monitor.Workspace.ReportDate.ToString("yyyy-MM-dd"))
                .Replace("{workspace}", monitor.Workspace.WorkspaceName)
                .Replace("{monitor}", monitor.MonitorName).Replace("{installation}", dwhInstallation); ;

  
          //  Console.WriteLine(preparedSqlQuery);

            DataTable dataTable = EtrmDbTool.GetDataTableFromDwh(server, preparedSqlQuery);
            dataTable.TableName = monitor.MonitorName;

            return dataTable;

        }

        public XLWorkbook GetCompareWorkbook(int monitorId)
        {
            Monitor monitor = this.dataModel.Monitors.SingleOrDefault(x => x.MonitorId == monitorId);

            if(monitor==null) throw new ArgumentException("Cannot find monitor with id: "+monitorId);

            DataTable expectedDataTable = GetDataTableFromDwh(monitor, false);
            DataTable actualTable = GetDataTableFromDwh(monitor, true);
            DataTable testData = new DataTable();
            testData.Columns.Add("Key", typeof(string));
            testData.Columns.Add("Value", typeof(string));
 

            testData.Rows.Add("PreTest", monitor.Workspace.PreTest.Name);
            testData.Rows.Add("Workspace", monitor.Workspace.WorkspaceName);
            testData.Rows.Add("Monitor", monitor.MonitorName);
            testData.Rows.Add("Tollerance", monitor.Tollerance);
            string prodversionName="";
            ProdEnvRun  prodversion = monitor.Workspace.ProdEnvRuns.LastOrDefault();
            if (prodversion != null)
            {
                prodversionName = prodversion.EtrmVersion;
            }

            testData.Rows.Add("Prod version", prodversionName);

            string testVersionName = "";
            TestEnvRun testRun = monitor.Workspace.TestEnvRuns.LastOrDefault();
            if (testRun != null)
            {
                testVersionName = testRun.EtrmVersion;
            }

            testData.Rows.Add("Test version", testVersionName);

            ClosedXmlTool closedXmlTool=new ClosedXmlTool("Pre Test");

            closedXmlTool.ExportDataTable(testData, 1, 1, XLColor.NoColor, XLColor.NoColor);

            int startRowActual = testData.Rows.Count + 6;
            int startColExpected = expectedDataTable.Columns.Count + 1;
            int startColCompare = expectedDataTable.Columns.Count*2 + 1;

            closedXmlTool.ExportDataTable(actualTable,startRowActual,1,XLColor.Yellow,XLColor.Yellow);
            closedXmlTool.ExportDataTable(expectedDataTable,startRowActual,startColExpected,XLColor.LightGreen,XLColor.LightGreen);

            DataTable emptyTable = expectedDataTable.Clone();
            closedXmlTool.ExportDataTable(emptyTable, startRowActual, startColCompare, XLColor.Red, XLColor.NoColor);
        



            int t = int.Parse(monitor.Tollerance.ToString("E").Split('-').Last());

           

            int formulaRow = startRowActual + 1;
            int formulaCol = startColCompare;

            int actualCol = 1;
            int expectedCol = startColExpected;

            for (int i = 0; i < actualTable.Rows.Count; i++)
            {
                for (int j = 0; j < actualTable.Columns.Count; j++)
                {
                    string actualStartCellAdress = closedXmlTool.GetAddress(formulaRow+i, actualCol+j);
                    string expectedStartCellAdress = closedXmlTool.GetAddress(formulaRow+i, expectedCol+j);

                    string formula = @"=IF(" + actualStartCellAdress + @"<>" + expectedStartCellAdress + @",IF(AND(ABS(" +
                                     actualStartCellAdress + @")<0.0000001,ABS(" + expectedStartCellAdress +
                                     @")<0.0000001),"""",ROUND(" + actualStartCellAdress + @"/" +
                                     expectedStartCellAdress + @"-1," + t + @" )),"""")";

                    closedXmlTool.AssignFormulaCell(formulaRow+i, formulaCol+j, formula);
                }

                
            }



            //closedXmlTool.AssignFormulaToRange(startRow+1,startColCompare,actualTable.Rows.Count,actualTable.Columns.Count,formula);
            //this.AssignFormulaToRange(startRow + 1, 1 + numActualColumns + numExpectedColumns, numberOfRows, numExpectedColumns, formula);


            return closedXmlTool.Workbook;
        }


        public void RunWorkspaceInTestEnvAndCompare(int workspaceId)
        {
            Workspace workspace = this.dataModel.Workspaces.Single(x => x.WorkspaceId == workspaceId);
            // this.dataModel = pretestModelEntities;
            int testRunId= RunWorkspaceTestEnv(workspace.WorkspaceId);
            this.CompareTestRun(testRunId);

        }

        public void CompareTestRun(int testRunId)
        {
            TestEnvRun testEnvRun = this.dataModel.TestEnvRuns.Single(x => x.TestEnvRunId == testRunId);
            if (testEnvRun.JobResult != "Success") return;

            IEnumerable<TestResult> prevResults = testEnvRun.TestResults;

            this.dataModel.TestResults.RemoveRange(prevResults);

            this.ValidateAndSaveDataModel();


            IEnumerable<Monitor> monitors = testEnvRun.Workspace.Monitors;

            foreach (Monitor monitor in monitors)
            {


                TestResult result = new TestResult
                {
                    TestResultStatus = "Success",
                    Monitor = monitor,

                };

                try
                {
                    DataTable expectedDataTable = GetDataTableFromDwh(monitor, false);
                    DataTable actualTable = GetDataTableFromDwh(monitor, true);

                    ComplexTypeAssert.AssertDataTables(expectedDataTable, actualTable);
                }
                catch (Exception ex)
                {

                    result.TestResultStatus = "Failure";
                    result.TestResultMessage = ex.Message;
                }

                testEnvRun.TestResults.Add(result);

                this.ValidateAndSaveDataModel();

            }
        }

        public int RunWorkspaceTestEnv(int workspaceId)
        {
            Workspace workspace = GetWorkspace(workspaceId);
            string serverName = workspace.PreTest.TestServer.ServerName;

            string etrmVersion = QaLookUpClient.GetEtrmVersion(serverName);

            DateTime startTime = DateTime.Now;

            TestEnvRun run = new TestEnvRun
            {
                Workspace = workspace,
                EtrmVersion = etrmVersion,
                RunStartTime = startTime,
                JobResult = "Running"
            };

            workspace.IsRunningInTestEnv = true;
            this.dataModel.Entry(workspace).State = EntityState.Modified;
            this.ValidateAndSaveDataModel();

            JobExecutionStatus status = ExceucteJob(workspace.PreTest.TestServer, workspace);

            if (status == null)
            {
                run.JobResult = "Time out";
            }
            else
            {
                int jobExecutionId = status.ExecutionId;
                string log = JobAPI.GetJobExecutionLog(jobExecutionId, serverName);

                run.JobResult = status.Status;
                run.TestServerJobLog = log;
            }

            workspace.IsRunningInTestEnv = false;
            this.dataModel.Entry(workspace).State = EntityState.Modified;
            this.dataModel.TestEnvRuns.Add(run);

            this.ValidateAndSaveDataModel();

            return run.TestEnvRunId;
        }

        public int RunWorkspaceProdEnv(int workspaceId)
        {
            Workspace workspace = GetWorkspace(workspaceId);

            string serverName = workspace.PreTest.ProdServer.ServerName;

            string etrmVersion = QaLookUpClient.GetEtrmVersion(serverName);

            DateTime startTime = DateTime.Now;

            ProdEnvRun run = new ProdEnvRun
            {
                Workspace = workspace,
                EtrmVersion = etrmVersion,
                RunStartTime = startTime

            };

            workspace.IsRunningInProdEnv = true;
            this.dataModel.Entry(workspace).State = EntityState.Modified;
            this.ValidateAndSaveDataModel();


            this.ValidateAndSaveDataModel();

            JobExecutionStatus status = ExceucteJob(workspace.PreTest.ProdServer, workspace);

         

            if (status == null)
            {
                run.JobResult = "Time out";
            }
            else
            {
                int jobExecutionId = status.ExecutionId;
                string log = JobAPI.GetJobExecutionLog(jobExecutionId, serverName);

                run.JobResult = status.Status;
                run.ProdServerLog = log;
            }

            workspace.IsRunningInProdEnv = false;
            this.dataModel.Entry(workspace).State = EntityState.Modified;
            this.dataModel.ProdEnvRuns.Add(run);

            this.ValidateAndSaveDataModel();

            return run.ProdEnvRunId;
        }

        public void Dispose()
        {
            this.dataModel.Dispose();
        }
    }
}