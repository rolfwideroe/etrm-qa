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
    public class TestResultsController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();

        // GET: TestResults
        public ActionResult Index(int testRunId)
        {
            try
            {
                IEnumerable<TestResult> testResults =
                    db.TestResults.Include(t => t.Monitor)
                        .Include(t => t.TestEnvRun)
                        .Where(x => x.TestEnvRunId == testRunId);

                ViewBag.WorkspaceName = db.TestEnvRuns.Find(testRunId).Workspace.WorkspaceName;
                ViewBag.PreTestName = db.TestEnvRuns.Find(testRunId).Workspace.PreTest.Name;
                ViewBag.PreTestId = db.TestEnvRuns.Find(testRunId).Workspace.PreTest.PreTestId;
                ViewBag.WorkspaceId = db.TestEnvRuns.Find(testRunId).Workspace.WorkspaceId;

                return View(testResults.ToList());
            }
            catch (Exception ex)
            {

                return RedirectToAction("Oops", "ErrorPage", new {@errorMessage = ex});

            }
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
