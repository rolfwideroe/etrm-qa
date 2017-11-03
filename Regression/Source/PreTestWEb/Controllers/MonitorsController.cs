using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using PreTestWeb.Managers;
using PreTestWeb.Models;

namespace PreTestWeb.Controllers
{
    public class MonitorsController : Controller
    {
        private PretestModelEntities db = new PretestModelEntities();

        // GET: Monitors
        public ActionResult Index(int workspaceId)
        {
            var monitors = db.Monitors.Include(m => m.MonitorType).Include(m => m.Workspace).Where(x=>x.WorkspaceId==workspaceId);
            return View(monitors.ToList());
        }

    

      

  

 


        // GET: Monitors/Edit/5
        public ActionResult EditQuery(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            ViewBag.MonitorTypeId = new SelectList(db.MonitorTypes, "MonitorTypeId", "MonitorTypeName", monitor.MonitorTypeId);
            ViewBag.WorkspaceId = new SelectList(db.Workspaces, "WorkspaceId", "WorkspaceName", monitor.WorkspaceId);
            return View(monitor);
        }

        // POST: Monitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuery([Bind(Include = "MonitorId,WorkspaceId,MonitorTypeId,MonitorName,Query,Tollerance")] Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitor).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", new { workspaceId = monitor.WorkspaceId});
            }
        //    ViewBag.MonitorTypeId = new SelectList(db.MonitorTypes, "MonitorTypeId", "MonitorTypeName", monitor.MonitorTypeId);
         //   ViewBag.WorkspaceId = new SelectList(db.Workspaces, "WorkspaceId", "WorkspaceName", monitor.WorkspaceId);
            return View(monitor);
        }

        // GET: Monitors/Delete/5
        public ActionResult ExportComparison(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return RedirectToAction("Oops", "ErrorPage", new { errorMessage = "Could not find Monitor with id: "+id });
            }

            try
            {
                using (PreTestManager preTestManager = new PreTestManager())
                {
                    XLWorkbook workbook = preTestManager.GetCompareWorkbook(monitor.MonitorId);

                    string fileName = monitor.MonitorName + ".xlsx";

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    // Response.ContentType = "application/excel";
                    //   Response.ContentType = "application/excel";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    using (MemoryStream myMemoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(myMemoryStream);
                        myMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }

                return RedirectToAction("Index", new {@workspaceId = monitor.WorkspaceId});
            }
            catch (Exception ex)
            {

                return RedirectToAction("Oops", "ErrorPage",new {errorMessage = ex});
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
