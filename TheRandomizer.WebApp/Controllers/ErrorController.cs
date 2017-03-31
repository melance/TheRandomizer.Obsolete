using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheRandomizer.WebApp.Controllers
{
    public class ErrorController : Controller
    {

        private List<string> _criticalFailures = new List<string>()
        {
            "You have been eaten by a gru!",
            "You reach out to push the orc off the bridge but instead lightly caress his back.  He is uncomfortable.",
            "You light your torch and accidentally light yourself on fire as well.  You are immune to cold for the rest of your life.",
            "You attempt to sneak quietly out the front door. Instead, you barrel out of a second story window screaming into the center of a group of goblins.",
            "You swing your mighty battle axe at the kobold and miss so terribly that the other kobols stop fighting and just walk away shaking their heads."
        };

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            ViewBag.Message = RandomMessage();
            return View();
        }

        private string RandomMessage()
        {
            var random = new Random();
            var index = random.Next(0, _criticalFailures.Count);
            return _criticalFailures[index];
        }
    }
}