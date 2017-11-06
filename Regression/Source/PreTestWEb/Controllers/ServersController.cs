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
    public class ServersController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();

        // GET: Servers
        public ActionResult Index()
        {
            var servers = db.Servers.Include(s => s.SqlServer);
            return View(servers.ToList());
        }

       

        // GET: Servers/Create
        public ActionResult Create()
        {
            ViewBag.SqlServerId = new SelectList(db.SqlServers, "SqlServerId", "SqlServerName");
            return View();
        }

        // POST: Servers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServerId,SqlServerId,ServerName,EcmDb,SystemDb,Dwh")] Server server)
        {
            if (ModelState.IsValid)
            {
                if (db.Servers.FirstOrDefault(x => x.ServerName == server.ServerName) == null)
                {
                    db.Servers.Add(server);
                    db.SaveChanges();

                }
                else
                {
                    ModelState.AddModelError("", "Error: server name already exist.");
                    ViewBag.SqlServerId = new SelectList(db.SqlServers, "SqlServerId", "SqlServerName");
                    return View(server);
                }
            }
            return RedirectToAction("Index");


                //ViewBag.SqlServerId = new SelectList(db.SqlServers, "SqlServerId", "SqlServerName", server.SqlServerId);
                //return View(server);
        }

        // GET: Servers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            ViewBag.SqlServerId = new SelectList(db.SqlServers, "SqlServerId", "SqlServerName", server.SqlServerId);
            return View(server);
        }

        // POST: Servers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServerId,SqlServerId,ServerName,EcmDb,SystemDb,Dwh")] Server server)
        {
            if (ModelState.IsValid)
            {
                db.Entry(server).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SqlServerId = new SelectList(db.SqlServers, "SqlServerId", "SqlServerName", server.SqlServerId);
            return View(server);
        }

        // GET: Servers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        // POST: Servers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Server server = db.Servers.Find(id);
            db.Servers.Remove(server);
            db.SaveChanges();
            return RedirectToAction("Index");
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
