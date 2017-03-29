using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheRandomizer.WebApp.Models;
using TheRandomizer.Generators;

namespace TheRandomizer.WebApp.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private SearchModel _defaultCriteria;

        public static List<SelectListItem> GetAuthors()
        {
            var authorList = new List<string>(DataAccess.DataContext.GetAuthors());
            var items = new List<SelectListItem>() { new SelectListItem() { Text = "", Value = "" } };
            foreach (var author in authorList)
            {
                items.Add(new SelectListItem() { Text = author, Value = author });
            }
            return items;
        }

        public static List<string> GetTags()
        {
            return new List<string>(DataAccess.DataContext.GetAllTags().Keys);
        }

        private SearchModel CriteriaDefaults
        {
            get
            {
                if (_defaultCriteria == null)
                {
                    _defaultCriteria = new SearchModel();
                    _defaultCriteria.Tags = new Dictionary<string, bool>(DataAccess.DataContext.GetAllTags());
                    _defaultCriteria.Page = 1;
                    _defaultCriteria.PageSize = 10;
                    _defaultCriteria.FavoritesOnly = false;
                }
                return _defaultCriteria;
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
            if (GetFormValue<Int32>("PageSize", criteria.PageSize) != criteria.PageSize)
            {
                criteria.Page = CriteriaDefaults.Page;
            }
            else
            {
                criteria.Page = GetFormValue<Int32>("Page", CriteriaDefaults.Page);
            }
            criteria.PageSize = GetFormValue<Int32>("PageSize", CriteriaDefaults.PageSize);
            criteria.FavoritesOnly = GetFormValue<bool>("FavoritesOnly", CriteriaDefaults.FavoritesOnly);
            
            foreach (var tag in Request.Form.AllKeys.Where(k => k.StartsWith("tag")))
            {
                var tagName = tag.Remove(0, 3);
                if (criteria.Tags.ContainsKey(tagName))
                {
                    criteria.Tags[tagName] = true;
                }
            }

            criteria = DataAccess.DataContext.Search(criteria);
            return View(criteria);
        }

        public ActionResult Generate(Int32 id)
        {
            var generator = DataAccess.DataContext.GetGenerator(id);
            if (generator != null)
            {
                var user = DataAccess.DataContext.User;
                var model = new GenerateModel()
                {
                    Id = id,
                    Name = generator.Name,
                    Author = generator.Author,
                    Description = generator.Description,
                    Parameters = generator.Parameters,
                    SupportsMaxLength = generator.SupportsMaxLength,
                    IsFavorite = user == null ? false : user.Favorites.Contains(id),
                    IsOwner = user == null ? false : user.OwnerOfGenerator.Contains(id)
                };
                ViewBag.Title = generator.Name;
                return View(model);
            }
            else
            {
                return new HttpNotFoundResult("Unable to find the requested generator");
            }
        }

        [HttpPost]
        public ActionResult Generate(GenerateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var generator = DataAccess.DataContext.GetGenerator(model.Id);
                    ViewBag.Title = generator.Name;
                    if (model.Parameters != null)
                    {
                        foreach (var parameter in model.Parameters)
                        {
                            generator.Parameters[parameter.Name].Value = parameter.Value;
                        }
                    }
                    var results = generator.Generate(model.Repeat, model.MaxLength);
                    return PartialView("_Results", results);
                }
                catch (Exception ex)
                {
                    return Content($"An error occured trying to perform the generation. <br /> {ex.Message}", "text/html");
                }
            }
            else
            {
                return Content("There was an error parsing the input.");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SetFavorite()
        {
            var id = Int32.Parse(Request.Form["id"]);
            var isFavorite = bool.Parse(Request.Form["isFavorite"]);
            return Json(DataAccess.DataContext.SetFavorite(id, isFavorite));
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