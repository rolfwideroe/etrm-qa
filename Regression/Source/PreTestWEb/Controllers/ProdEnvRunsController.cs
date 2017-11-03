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
    public class ProdEnvRunsController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();

        // GET: ProdEnvRuns
        public ActionResult Index(int workspaceId)
        {
            var prodEnvRuns = db.ProdEnvRuns.Include(p => p.Workspace).Where(w=>w.WorkspaceId==workspaceId).OrderByDescending(x=>x.RunStartTime);

            ViewBag.WorkspaceName = db.Workspaces.Find(workspaceId).WorkspaceName;
            ViewBag.PreTestName = db.Workspaces.Find(workspaceId).PreTest.Name;
            ViewBag.PreTestId = db.Workspaces.Find(workspaceId).PreTest.PreTestId;

            return View(prodEnvRuns.ToList());
        }

        // GET: ProdEnvRuns/Details/5
        public ActionResult Log(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdEnvRun prodEnvRun = db.ProdEnvRuns.Find(id);
            if (prodEnvRun == null)
            {
                return HttpNotFound();
            }
            return View(prodEnvRun);
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
