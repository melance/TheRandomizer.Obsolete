using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheRandomizer.WebApp.Controllers
{
    public class ErrorController : Controller
    {

        private List<KeyValuePair<string, string>> _criticalFailures = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("I wander into the darkened cave.", "You have been eaten by a gru!"),
            new KeyValuePair<string, string>("I move to push the orc off the bridge.", "You reach out to push the orc off the bridge but instead lightly caress his back.  He is uncomfortable."),
            new KeyValuePair<string, string>("I light my torch using fire bolt.", "You  accidentally light yourself on fire as well.  You are immune to cold for the rest of your life."),
            new KeyValuePair<string, string>("I sneak out of the window quietly.", "You barrel out of a second story window screaming into the center of a group of goblins."),
            new KeyValuePair<string, string>("I attack the kobold!", "You swing your mighty battle axe at the kobold and miss so terribly that the other kobolds stop fighting and just walk away shaking their heads.")
        };

        public ActionResult NotFound()
        {
            var message = RandomMessage();
            Response.StatusCode = 404;
            ViewBag.Action = message.Key;
            ViewBag.Result = message.Value;
            return View();
        }

        private KeyValuePair<string, string> RandomMessage()
        {
            var random = new Random();
            var index = random.Next(0, _criticalFailures.Count);
            return _criticalFailures[index];
        }
    }
}