using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiteDB;
using TheRandomizer.Generators;
using TheRandomizer.Utility;
using TheRandomizer.WebApp.Models;

namespace TheRandomizer.WebApp.DataAccess
{
    internal static class DataContext
    {
        private const string DB_PATH = "~/DataAccess/TheRandomizer.db";
        private const string GENERATORS_COLLECTION = "Generators";

        public static BsonMapper CreateMapper()
        {
            var mapper = BsonMapper.Global;
            mapper.Entity<BaseGenerator>()
                .Index(x => x.Name, true) // Unique Index on Generator Name
                .Id(x => x.Id, true); // primary key on Generator Id 
            return mapper;
        }

        public static LiteDatabase OpenDatabase()
        {
            return new LiteDatabase(HttpContext.Current.Server.MapPath(DB_PATH), CreateMapper());
        }
        

        public static BaseGenerator UpsertGenerator(BaseGenerator generator)
        {
            using (var db = OpenDatabase())
            {
                var collection = db.GetCollection<BaseGenerator>(GENERATORS_COLLECTION);
                if (collection.Upsert(generator))
                {
                    return collection.FindOne(bg => bg.Name == generator.Name);
                }
            }
            return null;
        }

        public static void DeleteGenerator(Int32 id)
        {
            using (var db = OpenDatabase())
            {
                var collection = db.GetCollection<BaseGenerator>(GENERATORS_COLLECTION);
                collection.Delete(g => g.Id == id);
            }
        }

        public static SearchModel Search(SearchModel criteria)
        {
            using (var db = OpenDatabase())
            {
                //TODO: Add select favorites only
                var tags = criteria.Tags.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
                var collection = db.GetCollection<BaseGenerator>(GENERATORS_COLLECTION)
                    .FindAll()
                    .Select(bg => bg.AsGeneratorInfo())
                    .OrderBy(bg => bg.Name)
                    .Where(bg => (bg.Tags == null || bg.Tags.Count == 0 || bg.Tags.Intersect(tags).Count() == tags.Count()) && (criteria.Name == null || bg.Name.IndexOf(criteria.Name, StringComparison.CurrentCultureIgnoreCase) >= 0));

                criteria.TotalResults = collection.Count();
                criteria.TotalPages = (Int32)Math.Ceiling((double)collection.Count() / criteria.PageSize);
                criteria.Results = collection.Pagination(criteria.Page, criteria.PageSize).ToList();
            }

            return criteria;
        }

        public static IDictionary<string, bool> GetAllTags()
        {
            using (var db = OpenDatabase())
            {
                var list = db.GetCollection<BaseGenerator>(GENERATORS_COLLECTION)
                                .FindAll()
                                .SelectMany(bg => bg.Tags)
                                .Distinct()
                                .ToList()
                                .ToDictionary(x => x, x => false);
                return list;
            }
        }
    }
}