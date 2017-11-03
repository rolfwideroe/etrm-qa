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
    public class PreTestsController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();
        // GET: PreTests
        public ActionResult Index()
        {
            //var preTests = db.PreTests.Include(p => p.ProdServer).Include(p => p.TestServer);
            //return View(preTests.ToList());

            var preTest = db.PreTests.Include(p => p.ProdServer).Include(p => p.TestServer).Include(p => p.Workspaces);

            return View(preTest.ToList());
        }

        // GET: PreTests/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PreTest preTest = db.PreTests.Find(id);
        //    if (preTest == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //var viewModel = new PretestModelEntities();
        //    //ViewBag.Workspaces = db.Workspaces.Where(x => x.PreTestId == id.Value);

        //    return View(preTest);
       // }

        // GET: PreTests/Create
        public ActionResult Create()
        {
            ViewBag.ProdServerId = new SelectList(db.Servers, "ServerId", "ServerName");
            ViewBag.TestServerId = new SelectList(db.Servers, "ServerId", "ServerName");
            return View();
        }

        // POST: PreTests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PreTestId,Name,ProdServerId,TestServerId,Active,Tollerance")] PreTest preTest)
        {
            if (ModelState.IsValid)
            {
                db.PreTests.Add(preTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProdServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.ProdServerId);
            ViewBag.TestServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.TestServerId);
            return View(preTest);
        }

        // GET: PreTests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreTest preTest = db.PreTests.Find(id);
            if (preTest == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProdServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.ProdServerId);
            ViewBag.TestServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.TestServerId);
            return View(preTest);
        }

        // POST: PreTests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreTestId,Name,ProdServerId,TestServerId,Active,Tollerance")] PreTest preTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(preTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProdServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.ProdServerId);
            ViewBag.TestServerId = new SelectList(db.Servers, "ServerId", "ServerName", preTest.TestServerId);
            return View(preTest);
        }

        // GET: PreTests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreTest preTest = db.PreTests.Find(id);
            if (preTest == null)
            {
                return HttpNotFound();
            }
            return View(preTest);
        }

        private  async Task RunPreTestAsync(CancellationToken token, int preTestId)
        {

            using (PreTestManager man = new PreTestManager())
            {
                man.RunPreTestInTestEnv(preTestId);
                // man.RunPreTestInTestEnv(token, preTestId);
            }

        }

        //private void Long(int preTestId)
        //{
        //    PretestModelEntities entities=new PretestModelEntities();

        //    IEnumerable<Workspace> workspaces =entities.PreTests.Single(x=>x.PreTestId==preTestId).Workspaces;

        //    foreach (Workspace workspace in workspaces)
        //    {
        //        manager.RunWorkspaceInTestEnvAndCompare(workspace, entities);
        //    }
        //}

        public ActionResult Run(int? id)
        {
            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreTest preTest = db.PreTests.Find(id);
            if (preTest == null)
            {
                return HttpNotFound();
            }



            HostingEnvironment.QueueBackgroundWorkItem(ct => RunPreTestAsync(ct, preTest.PreTestId));

         

            return RedirectToAction("Index");
            // return Action(preTest);
            //return View(preTest);
        }

        

        // POST: PreTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PreTest preTest = db.PreTests.Find(id);
            db.PreTests.Remove(preTest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateWorkspace(int id)
        {
            return RedirectToAction("Create", "Workspaces", new { @preTestId = id});
        }

        public ActionResult NavigateToWorkspaces(int id)
        {
            return RedirectToAction("Index", "Workspaces", new { @preTestId = id });
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
