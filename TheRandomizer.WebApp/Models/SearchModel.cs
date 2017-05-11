using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TheRandomizer.Generators;
using TheRandomizer.WebApp.DataAccess;

namespace TheRandomizer.WebApp.Models
{
	public class SearchModel
	{
		public Dictionary<string, bool> Tags { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		[Display(Name = "Favorites Only")]
		public bool FavoritesOnly { get; set; }
		[Display(Name = "Include Libraries")]
		public bool IncludeLibraries { get; set; }
		[Display(Name = "Generators Open In")]
		public bool OpenNewTab { get; set; }
		public List<SearchResult> Results { get; set; }
		public Int32 First { get; set; }
		public Int32 Last { get; set; }
	}

	public class SearchResult : GeneratorInfo
	{
		public SearchResult() : base() { }
		public SearchResult(BaseGenerator generator) 
		{
			var info = generator.AsGeneratorInfo();
			Id = info.Id;
			Name = info.Name;
			Description = info.Description;
			Published = info.Published;
			Author = info.Author;
            IsLibrary = info.IsLibrary;
            OutputFormat = info.OutputFormat;
            SupportsMaxLength = info.SupportsMaxLength;
			Tags.AddRange(info.Tags);            

			var user = DataContext.User;

			if (user != null)
			{
				IsFavorite = DataContext.User.Favorites.Contains(Id);
				IsOwner = DataContext.User.OwnerOfGenerator.Contains(Id);
			}
			else
			{
				IsFavorite = false;
				IsOwner = false;
			}

		}

		public bool IsFavorite { get; set; } = false;
		public bool IsOwner { get; set; } = false;
	}
}