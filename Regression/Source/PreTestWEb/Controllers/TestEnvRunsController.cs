using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PreTestWeb.Models;

namespace PreTestWeb.Controllers
{
    public class TestEnvRunsController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();

        // GET: TestEnvRuns
        public ActionResult Index(int workspaceId)
        {
            var testEnvRuns = db.TestEnvRuns.Include(t => t.Workspace).Where(w=>w.WorkspaceId==workspaceId).OrderByDescending(x=>x.RunStartTime);

            ViewBag.WorkspaceName = db.Workspaces.Find(workspaceId).WorkspaceName;
            ViewBag.PreTestName = db.Workspaces.Find(workspaceId).PreTest.Name;
            ViewBag.PreTestId = db.Workspaces.Find(workspaceId).PreTest.PreTestId;

            return View(testEnvRuns.ToList());
        }

        // GET: TestEnvRuns/Details/5
        public ActionResult Log(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestEnvRun testEnvRun = db.TestEnvRuns.Find(id);
            if (testEnvRun == null)
            {
                return HttpNotFound();
            }
            return View(testEnvRun);
        }

        
        public ActionResult NavigateToTestResults(int id)
        {
            return RedirectToAction("Index", "TestResults", new { testRunId = id });
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
