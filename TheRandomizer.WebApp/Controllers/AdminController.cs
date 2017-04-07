using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheRandomizer.WebApp.Models;
using static TheRandomizer.WebApp.HelperClasses.GlobalConstants;

namespace TheRandomizer.WebApp.Controllers
{
    [Authorize(Roles = ADMINISTRATOR_ROLE)]
    public class AdminController : Controller
    {
        ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }
        
        // Publish
        public ActionResult UnPublished()
        {
            var model = DataAccess.DataContext.GetUnpublished();
            return View(model);
        }
        
        public ActionResult Publish(Int32 id)
        {
            var model = DataAccess.DataContext.PublishGenerator(id);
            return View((object)model.Name);
        }

        // Assign Owner
        
        // Generator CRUD
    }
}