using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using PreTestWeb.Managers;
using PreTestWeb.Models;


namespace PreTestWeb.Controllers
{

    public class WorkspacesController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();
        private PreTestManager manager=new PreTestManager();



        // GET: Workspaces
        public ActionResult Index(int? preTestId)
        {
            if (preTestId == null)
            {
                
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "PreTest was not set for current workspace."});
              //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreTest preTest = db.PreTests.Find(preTestId);
            ViewBag.PreTestId = preTestId;
            ViewBag.PreTestName = preTest.Name;
            var workspaces = db.Workspaces.Where(w => w.PreTestId == preTestId.Value);
            return View(workspaces.ToList());
        }

        // GET: Workspaces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new {errorMessage="Workspace was not found."});
            }
            Workspace workspace = db.Workspaces.Find(id);
            if (workspace == null)
            {
                return HttpNotFound();
            }
            return View(workspace);
        }

        // GET: Workspaces/Create
        public ActionResult Create(int? preTestId)
        {
            if (preTestId == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "PreTest was not set for current workspace."});
            }
            
            

            IList<CommonWorkspace> commonWorkspaces = manager.GetCommonWorkspaces(preTestId.Value).ToList();
            ViewBag.PreTestName = db.PreTests.Find(preTestId).Name;
            ViewBag.WorkspaceName=new SelectList(commonWorkspaces, "DisplayName", "DisplayName");
            
            return View();
        }

        // POST: Servers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WorkspaceId,PreTestId,ReportDate,TimeOutInSeconds,WorkspaceName")] Workspace workspace)
        {
            
            string displayName = workspace.WorkspaceName;

            CommonWorkspace commonWorkspace =
                this.manager.GetCommonWorkspaces(workspace.PreTestId).Single(x => x.DisplayName == displayName);
            
            if (ModelState.IsValid)
            {
                try
                {
                    this.manager.CreateWorkspace(workspace.PreTestId, commonWorkspace, workspace.ReportDate,workspace.TimeOutInSeconds);
                }
                catch (Exception ex)
                {

                    return RedirectToAction("Oops", "ErrorPage", new { errorMessage = ex.Message});
                }
              

                return RedirectToAction("Index",new {preTestId=workspace.PreTestId});
            }

            ViewBag.PreTestId = new SelectList(db.PreTests, "PreTestId", "Name", workspace.PreTestId);
            return View(workspace);
        }

        // GET: Workspaces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Workspace was not found." });
            }
            Workspace workspace = db.Workspaces.Find(id);
            if (workspace == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreTestId = new SelectList(db.PreTests, "PreTestId", "Name", workspace.PreTestId);
            ViewBag.PreTestName = workspace.PreTest.Name;
            return View(workspace);
        }

        // POST: Workspaces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WorkspaceId,PreTestId,WorkspaceName,ReportDate,TimeOutInSeconds,ErmEcm,TimeOutInSeconds,IsRunningInProdEnv,IsRunningInTestEnv")] Workspace workspace)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workspace).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { @preTestId=workspace.PreTestId  });
            }
            ViewBag.PreTestId = new SelectList(db.PreTests, "PreTestId", "Name", workspace.PreTestId);
            return View(workspace);
        }

        // GET: Workspaces/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Workspace was not found." });
            }
            Workspace workspace = db.Workspaces.Find(id);
            if (workspace == null)
            {
                return HttpNotFound();
            }
            return View(workspace);
        }

        // POST: Workspaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Workspace workspace = db.Workspaces.Find(id);
            int preTestId = workspace.PreTestId;
            IEnumerable<PreTestWeb.Models.Monitor> monitors = db.Monitors.Where(x => x.WorkspaceId == workspace.WorkspaceId);

            IEnumerable<ProdEnvRun> prodEnvRuns =db.ProdEnvRuns.Where(x => x.Workspace.WorkspaceId == workspace.WorkspaceId);
            IEnumerable<TestEnvRun> testEnvRuns =db.TestEnvRuns.Where(x => x.Workspace.WorkspaceId == workspace.WorkspaceId);
            IEnumerable<TestResult> testResults =db.TestResults.Where(x => x.TestEnvRun.Workspace.WorkspaceId==workspace.WorkspaceId);

            db.Monitors.RemoveRange(monitors);
            db.ProdEnvRuns.RemoveRange(prodEnvRuns);
            db.TestEnvRuns.RemoveRange(testEnvRuns);
            db.TestResults.RemoveRange(testResults);

            db.Workspaces.Remove(workspace);

       

            db.SaveChanges();
            return RedirectToAction("Index", new { @preTestId = preTestId });
        }

        private async Task RunWorkspaceAndCompare(CancellationToken token, int workspaceId)
        {

            using (PreTestManager man = new PreTestManager())
            {
                man.RunWorkspaceInTestEnvAndCompare(workspaceId);
                // man.RunPreTestInTestEnv(token, preTestId);
            }

        }

        private async Task RunWorkspaceInProdEnv(CancellationToken token, int workspaceId)
        {

            using (PreTestManager man = new PreTestManager())
            {
                man.RunWorkspaceProdEnv(workspaceId);
            }

        }


        public ActionResult RunInTestEnvAndCompare(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Workspace was not found." });
            }

            Workspace workspace = db.Workspaces.Find(id.Value);

            if(workspace.IsRunningInTestEnv)
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "The Workspace:"+workspace.WorkspaceName +" is already running on Testserver: "+workspace.PreTest.TestServer.ServerName });

            HostingEnvironment.QueueBackgroundWorkItem(ct => RunWorkspaceAndCompare(ct, workspace.WorkspaceId));
            Thread.Sleep(100);
            return RedirectToAction("Index", new {@preTestId = workspace.PreTestId});
        }

        public ActionResult ReCompare(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Workspace was not found." });
            }

            Workspace workspace = db.Workspaces.Find(id.Value);

            TestEnvRun run = workspace.TestEnvRuns.LastOrDefault();

            if(run==null)
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "The Workspace: "+workspace.WorkspaceName+" on Reportdate: "+workspace.ReportDate.ToString("dd/MM/yyyy")+" has never been run in Test Env, please run it at least once before recomparing" });

            this.manager.CompareTestRun(run.TestEnvRunId);

            return RedirectToAction("Index", "TestResults", new {testRunId = run.TestEnvRunId});
        }

        public ActionResult RunInProdEnv(int? workspaceId)
        {
            if (workspaceId == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Workspace was not found." });
            }



            Workspace workspace = db.Workspaces.Find(workspaceId.Value);


            if (workspace.IsRunningInProdEnv)
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "The Workspace:" + workspace.WorkspaceName + " is already running on ProdServer: " + workspace.PreTest.ProdServer.ServerName });

            HostingEnvironment.QueueBackgroundWorkItem(ct => RunWorkspaceInProdEnv(ct, workspace.WorkspaceId));
            Thread.Sleep(100);
            return RedirectToAction("Index", new { @preTestId = workspace.PreTestId });
        }

        public ActionResult NavigateToMonitors(int id)
        {
            return RedirectToAction("Index","Monitors",  new { @workspaceId = id});
        }

        public ActionResult NavigateToTestEnvRuns(int id)
        {
            return RedirectToAction("Index", "TestEnvRuns", new { @workspaceId = id });
        }

        public ActionResult NavigateToProdEnvRuns(int workspaceId)
        {

            return RedirectToAction("Index", "ProdEnvRuns", new {workspaceId});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
