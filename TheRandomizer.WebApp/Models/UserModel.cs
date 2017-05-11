using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheRandomizer.WebApp.Models
{
    public class UserModel
    {

        public string Id { get; set; }
        public List<Guid> Favorites { get; set; } = new List<Guid>();
        public List<Guid> OwnerOfGenerator { get; set; } = new List<Guid>();
    }
}