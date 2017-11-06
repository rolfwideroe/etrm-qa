using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace PreTestWeb.Controllers
{
    public class ErrorPageController : Controller
    {
        // GET: ErrorPage
   

        public ActionResult Oops(string errorMessage)
        {
            //switch (id)
            //{
            //    case 1:
            //        ViewBag.Message = "error code 1";
            //        break;
            //    case 2:
            //        ViewBag.Message = "error code 2";
            //        break;
            //    default:
            //        ViewBag.Message = "error code common";
            //        break;
            //} 
            ViewBag.Message = errorMessage;
           // Response.StatusCode = 400;
            return View("Oops");
        }
    }
}