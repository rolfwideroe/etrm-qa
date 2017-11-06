using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using NUnit.Framework;
using PreTestWeb.Managers;
using PreTestWeb.Models;
using Monitor = PreTestWeb.Models.Monitor;

namespace PreTestWeb
{
    public class TestPreTestTool
    {
        public PretestModelEntities Datamodel=new PretestModelEntities();
        public int CreatePreTest(string preTestName, string prodServerName, string testServerName)
        {
            IEnumerable<Server> servers = Datamodel.Servers;


            Server prodServer = servers.Single(x => x.ServerName == prodServerName);
            Server testServer = servers.Single(x => x.ServerName == testServerName);

            PreTest preTest = new PreTest
            {
                TestServer = testServer,
                ProdServer = prodServer,
                Name = preTestName
            };

            Datamodel.PreTests.Add(preTest);
            Datamodel.SaveChanges();
            int pretestId = preTest.PreTestId;

            return pretestId;
        }

     

        public Workspace GetWorkspace(int workspaceId)
        {
            return this.Datamodel.Workspaces.Single(x => x.WorkspaceId == workspaceId);
        }

        public TestEnvRun GetTestEnvRun(int testEnvRunId)
        {
            return this.Datamodel.TestEnvRuns.Single(x => x.TestEnvRunId == testEnvRunId);
        }

        public ProdEnvRun GetProdEnvRun(int prodEnvRunId)
        {
            return this.Datamodel.ProdEnvRuns.Single(x => x.ProdEnvRunId == prodEnvRunId);
        }
    }

   
    public class TestPreTestManager
    {
   
        private readonly PreTestManager manager;
        private TestPreTestTool preTestTool;

        public TestPreTestManager()
        {
            this.preTestTool=new TestPreTestTool();
            this.manager=new PreTestManager();
        }

        private const string DefaultProdServerName = "NETVS-P21";
        private const string DefaultTestServerName = "NETVS-P20";

     

  

        [Test]
        public void TestGetCommonWorkspaces()
        {
            string preTestName = "TestGetCommonWorkspaces" + Guid.NewGuid();

            int preTestId = this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            IEnumerable<CommonWorkspace> ws = this.manager.GetCommonWorkspaces(preTestId);

            CommonWorkspace ecmCommonWorkspace = ws.Single(x => x.WorkspaceName == "Liquidity USD");
            Assert.AreEqual("Liquidity USD (ECM)",ecmCommonWorkspace.DisplayName);

            CommonWorkspace ermCommonWorkspace = ws.Single(x => x.WorkspaceName == "Exposicion GASPHYS");
            Assert.AreEqual("Exposicion GASPHYS (ERM)", ermCommonWorkspace.DisplayName);

            CommonWorkspace nonExistingWorkspace = ws.SingleOrDefault(x => x.WorkspaceName == "Not existing ws");
            Assert.IsNull(nonExistingWorkspace);
        }

      



        [Test]
        public void TestCreateWorkspace()
        {
            string preTestName = "TestCreateWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test Posmon";
            DateTime reportDate=new DateTime(2016,1,1);
            int timeOut = 4;

            Workspace workspace = CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, timeOut);

            Assert.AreNotEqual(0,workspace.WorkspaceId);
            Assert.AreEqual(workspaceName,workspace.WorkspaceName);
            Assert.AreEqual(reportDate,workspace.ReportDate);
            Assert.AreEqual(timeOut,workspace.TimeOutInSeconds);
            Assert.AreEqual("ECM",workspace.ErmEcm);

            IEnumerable<Monitor> monitors = workspace.Monitors;

            if(monitors==null||!monitors.Any()) Assert.Fail("Failed to creat Monitors");

            foreach (Monitor monitor in monitors)
            {
                Assert.AreEqual(monitor.MonitorType.DefaultQuery,monitor.Query);
            }
        }

        private Workspace CreateNewPreTestWithWorkspace(string preTestName, string workspaceName, DateTime reportDate, int timeOut)
        {
            int preTestId = this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(preTestId).Single(x => x.WorkspaceName == workspaceName);

            int workspaceId = this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timeOut);
            Workspace workspace = preTestTool.GetWorkspace(workspaceId);
            return workspace;
        }

        [Test]
        public void TestFailCreateDuplicateWorkspace()
        {
            string preTestName = "TestFailCreateDuplicateWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test Posmon";
            DateTime reportDate = new DateTime(2016, 1, 1);
            int timeOut = 4;

            int preTestId=this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);
            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(preTestId).Single(x => x.WorkspaceName == workspaceName);
       

            this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timeOut);

            string expectedError = "The Workspace: " + workspaceName + " with Reportdate: 01/01/2016 already exists for PreTest: " + preTestName;
            try
            {
                this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timeOut);
            }
            catch (Exception ex)
            {
                
                Assert.AreEqual(expectedError,ex.Message);
                return;
            }

            Assert.Fail("Expected to fail with error:\n"+expectedError);
           
        }




        [Test]
        public void TestCreateErmWorkspace()
        {
            string preTestName = "TestCreateErmWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test ERM";
            DateTime reportDate = new DateTime(2016, 1, 1);
            int timeOut = 4;

            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, timeOut);

            Assert.AreNotEqual(0, workspace.WorkspaceId);
            Assert.AreEqual(workspaceName, workspace.WorkspaceName);
            Assert.AreEqual(reportDate, workspace.ReportDate);
            Assert.AreEqual(timeOut, workspace.TimeOutInSeconds);
            Assert.AreEqual("ERM", workspace.ErmEcm);

            IEnumerable<Monitor> monitors = workspace.Monitors;

            if (monitors == null || !monitors.Any()) Assert.Fail("Failed to creat Monitors");

            string[] expectedMonitors = {
                "Cash flow at risk", "Value at risk analysis", "Sensitivity analysis",
                "Currency exposure report", "Exposure report"
            };

            Assert.AreEqual(expectedMonitors.Length,monitors.Count());

            foreach (string exp in expectedMonitors)
            {
                Monitor monitor = monitors.SingleOrDefault(x => x.MonitorName == exp);

                Assert.IsNotNull(monitor,"Expected monitor: "+exp+" was not saved");

                Assert.AreEqual(monitor.MonitorType.DefaultQuery, monitor.Query);
            }

         
        }

        [Test]
        public void TestExecuteWorkspaceTimeOut()
        {
            string preTestName = "TestExecuteWorkspaceTimeOut-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Posmon";

            DateTime reportDate=new DateTime(2016,09,27);

            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 1);

            int testEnvRunId= this.manager.RunWorkspaceTestEnv(workspace.WorkspaceId);

            TestEnvRun testEnvRun = this.preTestTool.GetTestEnvRun(testEnvRunId);

            Assert.AreNotEqual(0,testEnvRun.TestEnvRunId);
            
            Assert.AreEqual("Time out",testEnvRun.JobResult);
        }


        [Test]
        public void TestExcelExport()
        {
           

            string preTestName = "TestCreateWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test Posmon";
            DateTime reportDate = new DateTime(2016, 1, 1);
            int timeOut = 4;

            Workspace workspace = CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, timeOut);

            Monitor monitor = workspace.Monitors.Single();

        
            XLWorkbook book= this.manager.GetCompareWorkbook(monitor.MonitorId);

            Assert.IsNotNull(book);

        }

    
        [Test]
        public void TestExecuteWorkspaceInTestEnv()
        {
            string preTestName = "TestExecuteWorkspaceInTestEnv-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Posmon";

            DateTime reportDate = new DateTime(2016, 09, 27);


            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 600);

            int testEnvRunId =this.manager.RunWorkspaceTestEnv(workspace.WorkspaceId);
            TestEnvRun testEnvRun = this.preTestTool.GetTestEnvRun(testEnvRunId);
            Assert.AreEqual("Success", testEnvRun.JobResult, testEnvRun.TestServerJobLog);
            Assert.Greater(testEnvRun.TestServerJobLog.ToCharArray().Length,0);
        }

        [Test]
        public void TestExecuteWorkspaceInProdEnv()
        {
            string preTestName = "TestExecuteWorkspaceInPordEnv-" + Guid.NewGuid();
            const string workspaceName = "Pre Test Posmon";
            DateTime reportDate = new DateTime(2016, 09, 27);


            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60);

            int workspaceId = workspace.WorkspaceId;

            int prodEnvRunId= this.manager.RunWorkspaceProdEnv(workspaceId);
            ProdEnvRun prodEnvRun = this.preTestTool.GetProdEnvRun(prodEnvRunId);

            workspace = this.preTestTool.Datamodel.Workspaces.Single(x => x.WorkspaceId == workspaceId);

            Assert.AreEqual(false,workspace.IsRunningInProdEnv);

            Assert.AreEqual("Success", prodEnvRun.JobResult,prodEnvRun.ProdServerLog);
            Assert.Greater(prodEnvRun.ProdServerLog.ToCharArray().Length, 0);
        }

        [Test]
        public void TestExecuteWorkspaceAndCompare()
        {
            string preTestName = "TestExecuteWorkspaceAndCompare-" + Guid.NewGuid();

            const string workspaceName = "Pre Test CFMon";
                                      
            DateTime reportDate = new DateTime(2016, 09, 27);

            CreatePreTestRunWorkspaceAndCompareTestEnv(preTestName, workspaceName, reportDate);
        }

        [Test]
        public void TestExecuteWorkspaceAndCompareAndRecompare()
        {
            string preTestName = "TestExecuteWorkspaceAndCompare-" + Guid.NewGuid();

            const string workspaceName = "Pre Test CFMon";

            DateTime reportDate = new DateTime(2016, 09, 27);

            int workspaceId= this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60).WorkspaceId;
            this.RunWorkspaceAndCompareTestEnv(workspaceId);

            int testEnvRunId= this.preTestTool.Datamodel.TestEnvRuns.Single(x => x.WorkspaceId == workspaceId).TestEnvRunId;

            TestResult prevResult =this.preTestTool.Datamodel.TestResults.Single(x => x.TestEnvRunId == testEnvRunId);

            int prevResultId = prevResult.TestResultId;

            this.manager.CompareTestRun(testEnvRunId);

            TestResult newResult = this.preTestTool.Datamodel.TestResults.Single(x => x.TestEnvRunId == testEnvRunId);

            Assert.Greater(newResult.TestResultId,prevResultId);

        }

        [Test]
        public void TestExecuteWorkspaceAndCompareMultMonitor()
        {
            string preTestName = "TestExecuteWorkspaceAndCompareMultMonitor-" + Guid.NewGuid();

            const string workspaceName = "Pre Test MultMon";

            DateTime reportDate = new DateTime(2016, 09, 27);

            CreatePreTestRunWorkspaceAndCompareTestEnv(preTestName, workspaceName, reportDate);
        }

        //private async Task RunPreTestAsync(CancellationToken token, int preTestId)
        //{

        //    using (PreTestManager man = new PreTestManager())
        //    {
        //        man.RunPreTestInTestEnv(preTestId);
        //        // man.RunPreTestInTestEnv(token, preTestId);
        //    }

        //}

        //[ExportDataTable]
        public void TestExecuteInProdAndTestErmandCompareAsync()
        {
            string preTestName = "TestExecuteInProdAndTestErmandCompareAsync-" + Guid.NewGuid();

            const string workspaceName = "Pre ExportTest Exposure";

            DateTime reportDate = new DateTime(2016, 09, 27);

       

            Workspace workspace = CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60);

            int prodEnvRunId = this.manager.RunWorkspaceProdEnv(workspace.WorkspaceId);

            ProdEnvRun prodEnvRun = this.preTestTool.GetProdEnvRun(prodEnvRunId);

            //Task.Run(ct => RunPreTestAsync(ct, preTest.PreTestId));


            //Thread.Sleep(10);

            //Assert.IsNotNull(workspace.TestEnvRuns);



        }



        [Test]
        public void TestExecuteInProdAndTestErmandCompare()
        {
            string preTestName = "TestExecuteInProdAndTestErmandCompare-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Exposure";

            DateTime reportDate = new DateTime(2016, 09, 27);

            Workspace workspace = CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60);

            int prodEnvRunId= this.manager.RunWorkspaceProdEnv(workspace.WorkspaceId);

            ProdEnvRun prodEnvRun = this.preTestTool.GetProdEnvRun(prodEnvRunId);

           Assert.AreEqual("Success", prodEnvRun.JobResult, prodEnvRun.ProdServerLog);

            int workspaceId = workspace.WorkspaceId;

            this.RunWorkspaceAndCompareTestEnv(workspaceId);
        }




        //   [ExportDataTable]
        public void IberiaPreTestPosmon()
        {          
            const string workspaceName = "Pre ExportTest Posmon";
            string preTestName = workspaceName+"-" + Guid.NewGuid();

            DateTime reportDate = new DateTime(2016, 09, 27);

;
            

            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60);


            this.manager.RunWorkspaceTestEnv(workspace.WorkspaceId);

            TestEnvRun testEnvRun = workspace.TestEnvRuns.Single();
            Assert.AreEqual("Success", testEnvRun.JobResult,testEnvRun.TestServerJobLog);
            Assert.Greater(testEnvRun.TestServerJobLog.ToCharArray().Length, 0);

            IEnumerable<TestResult> results = testEnvRun.TestResults;

            foreach (TestResult result in results)
            {
                Assert.AreEqual("Success", result.TestResultStatus, "Result from Monitor: " + result.Monitor.MonitorName + " Failed\n" + result.TestResultMessage);
            }

        }

      ////  [ExportDataTable]
      //  public void PreTestFinland()
      //  {
      //      const string workspaceName = "PreTest";
      //      string preTestName = "PreTestFinland";

      //      DateTime reportDate = new DateTime(2016, 10, 21);

      //      if (this.manager.GetPreTests().SingleOrDefault(x => x.Name == preTestName) == null)
      //      {
      //          PreTest preTest = CreatePreTest(preTestName, "NETVS-P07", "NETVS-P10");

      //          this.CreateNewPreTestWithWorkspace(preTest, workspaceName, reportDate, 60 * 10);
      //      }

      //      Workspace workspace =
      //          this.manager.GetPreTests()
      //              .Single(x => x.Name == preTestName)
      //              .Workspaces.Single(x => x.WorkspaceName == workspaceName);


      //      this.managerAsync.RunWorkspaceProdEnv(workspace.WorkspaceId);
      //      this.managerAsync.RunWorkspaceTestEnv(workspace.WorkspaceId);

      //      TestEnvRun testEnvRun = workspace.TestEnvRuns.Single();
      //      Assert.AreEqual("Success", testEnvRun.JobResult, testEnvRun.TestServerJobLog);
      //      Assert.Greater(testEnvRun.TestServerJobLog.ToCharArray().Length, 0);

      //      IEnumerable<TestResult> results = testEnvRun.TestResults;

      //      foreach (TestResult result in results)
      //      {
      //          Assert.AreEqual("Success", result.TestResultStatus, "Result from Monitor: " + result.Monitor.MonitorName + " Failed\n" + result.TestResultMessage);
      //      }

      //  }


        private void CreatePreTestRunWorkspaceAndCompareTestEnv(string preTestName, string workspaceName, DateTime reportDate)
        {
         
            Workspace workspace = this.CreateNewPreTestWithWorkspace(preTestName, workspaceName, reportDate, 60);
            this.RunWorkspaceAndCompareTestEnv(workspace.WorkspaceId);
        }

        private void RunWorkspaceAndCompareTestEnv(int workspaceId)
        {
            using (PreTestManager async=new PreTestManager())
            {
                async.RunWorkspaceInTestEnvAndCompare(workspaceId);
            }



            TestEnvRun testEnvRun = this.preTestTool.Datamodel.TestEnvRuns.Single(x => x.WorkspaceId == workspaceId);
            Assert.AreEqual("Success", testEnvRun.JobResult,"ExportTest Run Failed \n"+testEnvRun.TestServerJobLog);
            Assert.Greater(testEnvRun.TestServerJobLog.ToCharArray().Length, 0);

            IEnumerable<TestResult> results = testEnvRun.TestResults;
            Assert.Greater(results.Count(),0);
            foreach (TestResult result in results)
            {
                if (result.TestResultStatus != "Success")
                    Assert.Fail("Result from Monitor: " + result.Monitor.MonitorName + " Failed\n" + result.TestResultMessage);
            }
        }

   


    }
}
