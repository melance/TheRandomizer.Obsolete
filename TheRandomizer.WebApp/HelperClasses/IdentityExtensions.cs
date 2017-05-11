using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using TheRandomizer.WebApp.Models;
using System.Security.Claims;

namespace TheRandomizer.WebApp.HelperClasses
{
    public static class IdentityExtensions
    {
        public static bool GetIsAdministrator(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsAdministrator");
            return (claim != null) ? bool.Parse(claim.Value) : false;
        }

        public static bool GetIsOwner(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsOwner");
            return (claim != null) ? bool.Parse(claim.Value) : false;
        }
    }
}