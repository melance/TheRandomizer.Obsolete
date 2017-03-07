using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheRandomizer.WebApp.Models;
using TheRandomizer.Generators;

namespace TheRandomizer.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private SearchModel CriteriaDefaults
        {
            get
            {
                var criteria = new SearchModel();
                // set the criteria defaults
                criteria.Tags = new Dictionary<string, bool>(DataAccess.DataContext.GetAllTags());
                criteria.Page = 1;
                criteria.PageSize = 10;
                criteria.FavoritesOnly = false;
                return criteria;
            }
        }
        
        public ActionResult Index()
        {
            var criteria = CriteriaDefaults;
            criteria = DataAccess.DataContext.Search(criteria);
            return View(criteria);
        }

        [HttpPost]
        public ActionResult Index(SearchModel criteria)
        {
            criteria.Tags = new Dictionary<string, bool>(DataAccess.DataContext.GetAllTags());
            criteria.Page = GetFormValue<Int32>("Page", 1);
            criteria.PageSize = GetFormValue<Int32>("PageSize", 10);
            criteria.FavoritesOnly = GetFormValue<bool>("FavoritesOnly", false);
            criteria = DataAccess.DataContext.Search(criteria);
            
            foreach (var tag in Request.Form.AllKeys.Where(k => k.StartsWith("tag")))
            {
                var tagName = tag.Remove(0, 3);
                if (criteria.Tags.ContainsKey(tagName))
                {
                    criteria.Tags[tagName] = true;
                }
            }
            return View(criteria);
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "The Randomizer is an always free and open source application designed with role-playing games and story tellers in mind. It is a customizable random thing generator able to generate nearly limitless things including and certainly not limited to names, plot hooks, and maps.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private T GetFormValue<T> (string key, T defaultValue)
        {
            if (Request.Form.AllKeys.Contains(key))
            {
                if (typeof(T) == typeof(bool))
                {
                    var value = false;
                    switch (Request.Form[key].ToLower())
                    {
                        case "true":
                        case "yes":
                        case "on": value = true; break;
                        case "false":
                        case "no":
                        case "off": value = false; break;
                    }
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return (T)Convert.ChangeType(Request.Form[key], typeof(T));
            }
            return defaultValue;
        }
    }
}