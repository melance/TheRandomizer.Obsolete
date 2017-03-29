using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheRandomizer.WebApp.Models
{
    public class UserModel
    {

        public string Id { get; set; }
        public List<Int32> Favorites { get; set; } = new List<Int32>();
        public List<Int32> OwnerOfGenerator { get; set; } = new List<Int32>();
        public List<Int32> OwnerOfLibrary { get; set; } = new List<Int32>();
    }
}