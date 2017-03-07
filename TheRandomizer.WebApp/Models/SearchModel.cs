using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TheRandomizer.Generators;

namespace TheRandomizer.WebApp.Models
{
	public class SearchModel
	{
		public Dictionary<string, bool> Tags { get; set; }
		public string Name { get; set; }
        [Display(Name = "Favorites Only")]
		public bool FavoritesOnly { get; set; }
        [Display(Name = "Page Size")]
		public Int32 PageSize { get; set; }
		public Int32 Page { get; set; }
        public List<GeneratorInfo> Results { get; set; }
        public Int32 TotalResults { get; set; }
        public Int32 TotalPages { get; set; }
	}
}