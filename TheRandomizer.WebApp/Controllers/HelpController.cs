using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheRandomizer.WebApp.Controllers
{
    public class HelpController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generators()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }

        public ActionResult Terminology()
        {
            return View();
        }
        
        public ActionResult Calculations()
        {
            return View();
        }

        public ActionResult Assignment()
        {
            return View();
        }
    }
}