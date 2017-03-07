using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheRandomizer.Generators.Assignment;
using TheRandomizer.Generators.Dice;
using TheRandomizer.Generators.List;
using TheRandomizer.Generators.LUA;
using TheRandomizer.Generators.Phonotactics;
using TheRandomizer.Generators.Parameter;
using TheRandomizer.Generators.Table;
using TheRandomizer.WebApp.Models;
using TheRandomizer.Generators;

namespace TheRandomizer.WebApp.Controllers
{
    public class UserContentController : Controller
    {
        List<GeneratorTypeModel> _generatorTypes = new List<GeneratorTypeModel>()
                    {
                        new GeneratorTypeModel(typeof(AssignmentGenerator), "AssignmentEditor"),
                        new GeneratorTypeModel(typeof(DiceGenerator), "DiceEditor"),
                        new GeneratorTypeModel(typeof(ListGenerator), "ListEditor"),
                        new GeneratorTypeModel(typeof(LUAGenerator), "LuaEditor"),
                        new GeneratorTypeModel(typeof(PhonotacticsGenerator), "PhonotacticsEditor"),
                        new GeneratorTypeModel(typeof(TableGenerator), "TableEditor")
                    };

        #region Base Generator
        public ActionResult CreateParameter()
        {
            var parameter = new Configuration(); 
            return PartialView("~/Views/Shared/EditorTemplates/Configuration.cshtml", parameter);
        }

        public ActionResult SelectGeneratorType()
        {                      
            return View(_generatorTypes);            
        }

        [HttpPost]
        public ActionResult SelectGeneratorType(HttpPostedFileBase upload)
        {
            if (upload == null || upload.ContentLength == 0)
            {
                // No or Empty File Error
                ViewBag.UploadError = "There was an error retrieving the file, either no file was found or the file was zero length.";
            }
            else if (upload.ContentType != "text/xml")
            {
                // Wrong content type error
                ViewBag.UploadError = $"The file provided was of the wrong content type, expcted \"text/xml\" but recieved \"{upload.ContentType}\" content is accepted";
            }
            else
            {
                try
                {
                    // Attempt to deserialize the generator
                    var length = (Int32)upload.InputStream.Length;
                    byte[] bytes = new byte[length];
                    string xml;
                    BaseGenerator model;
                    GeneratorTypeModel modelType;

                    upload.InputStream.Read(bytes, 0, length);

                    xml = System.Text.Encoding.UTF8.GetString(bytes);

                    model = BaseGenerator.Deserialize(xml);

                    // Open appropriate editor with these contents
                    modelType = _generatorTypes.Find(gtm => gtm.Type == model.GetType());

                    if (modelType == null)
                    {
                        ViewBag.UploadError = $"Unrecognized generator type: {model.GetType().Name}";
                    }
                    else
                    {
                        return View(modelType.Action, model);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.UploadError = $"There was an error trying to read the contents of the file. {ex.Message}";
                }
            }
            return View(_generatorTypes);
        }
        #endregion

        #region Assignment Generators
        public ActionResult AssignmentEditor()
        {
            return View(new AssignmentGenerator());
        }

        [HttpPost]
        public ActionResult AssignmentEditor(AssignmentGenerator generator)
        {
            var model = generator;
            if (Request.Form.AllKeys.Contains("Submit", StringComparer.CurrentCultureIgnoreCase) && Request.Form["Submit"].Equals("Save", StringComparison.CurrentCultureIgnoreCase))
            {
                if (ModelState.IsValid)
                {
                    model = (AssignmentGenerator)DataAccess.DataContext.UpsertGenerator(generator);
                }
            }
            
            return View(model);
        }

        public ActionResult CreateLineItem()
        {
            var item = new LineItem();
            return PartialView("~/Views/Shared/EditorTemplates/LineItem.cshtml", item);
        }
        #endregion 
    }
}
