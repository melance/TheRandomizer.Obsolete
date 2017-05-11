using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheRandomizer.WebApp.HelperClasses;
using static TheRandomizer.WebApp.HelperClasses.GlobalConstants;

namespace TheRandomizer.WebApp.Controllers
{
    [Authorize(Roles = OWNER_ROLE)]
    public class ElmahController : Controller
    {        
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }
    }
}