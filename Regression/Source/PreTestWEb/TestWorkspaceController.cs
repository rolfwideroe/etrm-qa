using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using PreTestWeb.Controllers;
using PreTestWeb.Managers;
using PreTestWeb.Models;

namespace PreTestWeb
{
    [TestFixture]
    public class TestWorkspaceController
    {
        readonly PretestModelEntities dataModel=new PretestModelEntities();
        TestPreTestTool preTestTool=new TestPreTestTool();
        PreTestManager manager = new PreTestManager();
        private const string DefaultProdServerName = "NETVS-P21";
        private const string DefaultTestServerName = "NETVS-P20";

        private void CreatePreTest(string preTestName, string prodServerName, string testServerName)
        {
            IEnumerable<Server> servers = this.dataModel.Servers;


            Server prodServer = servers.Single(x => x.ServerName == prodServerName);
            Server testServer = servers.Single(x => x.ServerName == testServerName);

            PreTest preTest = new PreTest
            {
                ProdServer = prodServer,
                TestServer = testServer,
                Name = preTestName
            };

            this.dataModel.PreTests.Add(preTest);

            this.dataModel.SaveChanges();
            
        }
        [Test]
        public void TestCreatePreTest()
        {
            PreTestsController controller = new PreTestsController();

            ViewResult view=controller.Create() as ViewResult;


            string name = "TestCreatePreTest" + Guid.NewGuid();

            PreTest preTest = new PreTest
            {
                TestServerId = this.dataModel.Servers.Single(x => x.ServerName == DefaultTestServerName).ServerId,
                ProdServerId = this.dataModel.Servers.Single(x => x.ServerName == DefaultProdServerName).ServerId,
                Active = true,
                Name = name
            };

           
            controller.Create(preTest);

            ViewResult indexView = controller.Index() as ViewResult;

            Assert.IsNotNull(indexView);

            IEnumerable<PreTest> preTests = indexView.Model as IEnumerable<PreTest>;

            Assert.IsNotNull(preTests);

            PreTest actualPreTest = preTests.Single(x => x.Name == name);

            Assert.AreEqual(preTest.ProdServerId, actualPreTest.ProdServerId);
            Assert.AreEqual(preTest.TestServerId, actualPreTest.TestServerId);
            Assert.AreEqual(preTest.Active, actualPreTest.Active);

        }

        [Test]
        public void TestGetPreTestNotExisting()
        {
            PreTestsController controller = new PreTestsController();
            ViewResult indexView = controller.Index() as ViewResult;
            Assert.IsNotNull(indexView);
            IEnumerable<PreTest> preTests = indexView.Model as IEnumerable<PreTest>;
            PreTest preTest = preTests.SingleOrDefault(x => x.Name == "ThisNameDoesNotExist");

            Assert.IsNull(preTest);
        }

        [Test]
        public void TestCreateWorkspace()
        {
            string preTestName = "TestCreateWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test Posmon";
            string workspaceDisplayName = "Pre Test Posmon (ECM)";
            DateTime reportDate = new DateTime(2016, 1, 1);
            int timeOut = 4;

            this.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

             int preTestId = this.dataModel.PreTests.Single(x => x.Name == preTestName).PreTestId;

            Workspace workspace = new Workspace
            {
                WorkspaceName = workspaceDisplayName,
                ReportDate = reportDate,
                TimeOutInSeconds = timeOut,
                PreTestId = preTestId
            };

            WorkspacesController controller=new WorkspacesController();

           RedirectToRouteResult result=controller.Create(workspace) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(preTestId, result.RouteValues["preTestId"]);

            ViewResult view=controller.Index(preTestId) as ViewResult;

            Assert.IsNotNull(view);

            IEnumerable<Workspace> actualWorkspaces = view.Model as IEnumerable<Workspace>;

            Assert.IsNotNull(actualWorkspaces);

            Workspace acutalWorkspace = actualWorkspaces.Single(x => x.WorkspaceName == workspaceName);

            Assert.IsNotNull(acutalWorkspace);

            Assert.AreNotEqual(0, acutalWorkspace.WorkspaceId);
            Assert.AreEqual(workspaceName, acutalWorkspace.WorkspaceName);
            Assert.AreEqual(reportDate, acutalWorkspace.ReportDate);
            Assert.AreEqual(timeOut, acutalWorkspace.TimeOutInSeconds);
            Assert.AreEqual("ECM", acutalWorkspace.ErmEcm);

            IEnumerable<Monitor> monitors = acutalWorkspace.Monitors;

            if (monitors == null || !monitors.Any()) Assert.Fail("Failed to creat Monitors");

            foreach (Monitor monitor in monitors)
            {
                Assert.AreEqual(monitor.MonitorType.DefaultQuery, monitor.Query);
            }
        }

        [Test]
        public void TestFailCreateDuplicateWorkspace()
        {

            WorkspacesController controller=new WorkspacesController();

            string preTestName = "TestFailCreateDuplicateWorkspace" + Guid.NewGuid();
            string workspaceName = "Pre Test Posmon";
            string workspaceDisplayName = "Pre Test Posmon (ECM)";
            DateTime reportDate = new DateTime(2016, 1, 1);
            int timeOut = 4;

            this.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            int preTestId = this.dataModel.PreTests.Single(x => x.Name == preTestName).PreTestId;

            Workspace workspace = new Workspace
            {
                PreTestId = preTestId,
                WorkspaceName = workspaceDisplayName,
                ReportDate = reportDate,
                TimeOutInSeconds = timeOut
            };

            RedirectToRouteResult result= controller.Create(workspace) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index",result.RouteValues["action"]);
            Assert.AreEqual(preTestId,result.RouteValues["preTestId"]);
            
            string expectedError = "The Workspace: " + workspaceName + " with Reportdate: 01/01/2016 already exists for PreTest: " + preTestName;

            RedirectToRouteResult errorResult = controller.Create(workspace) as RedirectToRouteResult;

            Assert.IsNotNull(errorResult);
            Assert.AreEqual("ErrorPage", errorResult.RouteValues["controller"]);
            Assert.AreEqual("Oops", errorResult.RouteValues["action"]);
            Assert.AreEqual(expectedError, errorResult.RouteValues["errorMessage"]);

   

        }

        [Test]
        public void TestFailIfAlreadyRunningInTest()
        {
            string preTestName = "TestFailIfAlreadyRunningInTest-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Posmon";

            DateTime reportDate = new DateTime(2016, 09, 27);
            int timout = 60;

            int preTestId= this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(preTestId).Single(x => x.WorkspaceName == workspaceName);

            int workspaceId = this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timout);

            WorkspacesController controller=new WorkspacesController();

            Workspace workspace= this.dataModel.Workspaces.Find(workspaceId);
            workspace.IsRunningInTestEnv = true;

            this.dataModel.Entry(workspace).State = EntityState.Modified;

            this.dataModel.SaveChanges();

            string expectedError = "The Workspace:" + workspace.WorkspaceName + " is already running on Testserver: " +
                                   workspace.PreTest.TestServer.ServerName;

            RedirectToRouteResult errorResult = controller.RunInTestEnvAndCompare(workspaceId) as RedirectToRouteResult;

            Assert.IsNotNull(errorResult);
            Assert.AreEqual("ErrorPage", errorResult.RouteValues["controller"]);
            Assert.AreEqual("Oops", errorResult.RouteValues["action"]);
            Assert.AreEqual(expectedError, errorResult.RouteValues["errorMessage"]);

            workspace.IsRunningInTestEnv = false;

            this.dataModel.Entry(workspace).State = EntityState.Modified;

            this.dataModel.SaveChanges();
        }

        [Test]
        public void TestFailIfAlreadyRunningInProd()
        {
            string preTestName = "TestFailIfAlreadyRunningInProd-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Posmon";

            DateTime reportDate = new DateTime(2016, 09, 27);
            int timout = 60;

            int preTestId = this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(preTestId).Single(x => x.WorkspaceName == workspaceName);

            int workspaceId = this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timout);

            WorkspacesController controller = new WorkspacesController();

            Workspace workspace = this.dataModel.Workspaces.Find(workspaceId);
            workspace.IsRunningInProdEnv = true;

            this.dataModel.Entry(workspace).State = EntityState.Modified;

            this.dataModel.SaveChanges();

            string expectedError = "The Workspace:" + workspace.WorkspaceName + " is already running on ProdServer: " +
                                   workspace.PreTest.ProdServer.ServerName;

            RedirectToRouteResult errorResult = controller.RunInProdEnv(workspaceId) as RedirectToRouteResult;

            Assert.IsNotNull(errorResult);
            Assert.AreEqual("ErrorPage", errorResult.RouteValues["controller"]);
            Assert.AreEqual("Oops", errorResult.RouteValues["action"]);
            Assert.AreEqual(expectedError, errorResult.RouteValues["errorMessage"]);

            workspace.IsRunningInProdEnv = false;

            this.dataModel.Entry(workspace).State = EntityState.Modified;

            this.dataModel.SaveChanges();
        }

        [Test]
        public void TestFailRecompare()
        {
            string preTestName = "TestFailRecompare-" + Guid.NewGuid();

            const string workspaceName = "Pre Test Posmon";

            DateTime reportDate = new DateTime(2016, 09, 27);
            int timout = 60;

            int preTestId = this.preTestTool.CreatePreTest(preTestName, DefaultProdServerName, DefaultTestServerName);

            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(preTestId).Single(x => x.WorkspaceName == workspaceName);

            int workspaceId = this.manager.CreateWorkspace(preTestId, commonWorkspace, reportDate, timout);

            WorkspacesController controller = new WorkspacesController();

            RedirectToRouteResult errorResult = controller.ReCompare(workspaceId) as RedirectToRouteResult;

            string expectedError = "The Workspace: Pre Test Posmon on Reportdate: 27/09/2016 has never been run in Test Env, please run it at least once before recomparing";

            Assert.IsNotNull(errorResult);
            Assert.AreEqual("ErrorPage", errorResult.RouteValues["controller"]);
            Assert.AreEqual("Oops", errorResult.RouteValues["action"]);
            Assert.AreEqual(expectedError, errorResult.RouteValues["errorMessage"]);

           
        }

        //[Test]
        public void T()
        {
            PreTestsController controller = new PreTestsController();

            ViewResult view = controller.Index() as ViewResult;

            IEnumerable<PreTest> info = view.Model as IEnumerable<PreTest>;

            foreach (var var in info)
            {
                Console.WriteLine(var.Name);
            }

            PreTest t = info.Single(x => x.Name == "Web test");

            controller.Run(t.PreTestId);

        }

       // [Test]
        public void WorkspaceTest()
        {
            WorkspacesController workspaceController = new WorkspacesController();

            //ViewResult view = controller.Index(202) as ViewResult;

            //IEnumerable<Workspace> info = view.Model as IEnumerable<Workspace>;

            //foreach (var var in info)
            //{
            //    Console.WriteLine(var.WorkspaceName);
            //}

            PreTestsController preTestsController=new PreTestsController();

            ViewResult view=preTestsController.CreateWorkspace(202) as ViewResult;

           
            Workspace ws=view.Model as Workspace;

            ws.WorkspaceName = "A";
            ws.ReportDate=new DateTime(2011,11,11);
            ws.TimeOutInSeconds = 666;

            workspaceController.Create(ws);


        }

    }
}