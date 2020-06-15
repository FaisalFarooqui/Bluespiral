using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueSpiral.Web.Areas.Supplier.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Supplier/Supplier
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Welcome()
        {
            return View();
        }
        
        public ActionResult Logout()
        {
            Session["UserEmail"] = null;
            Session["PhotoUrl"] = null;
            Session["Name"] = null;
            return RedirectToAction("Login", "Account", new { area = "" });

        }
    }
}