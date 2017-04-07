using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace TheRandomizer.WebApp.HelperClasses
{
    public class IsOwnerAttribute : AuthorizeAttribute
    {
        private List<string> OverridableRoles { get; set; } = new List<string>();

        public IsOwnerAttribute() { }
        public IsOwnerAttribute(params string[] overridableRoles)
        {
            OverridableRoles = new List<string>(overridableRoles);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result == null || filterContext.Result.GetType() != typeof(HttpUnauthorizedResult))
            {
                if (!IsInRole(filterContext.HttpContext) && !IsOwner(filterContext.HttpContext))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "NotAuthorized", controller = "User" }));
                }
            }
        }

        protected bool IsInRole(HttpContextBase httpContext)
        {
            foreach (var role in OverridableRoles)
            {
                if (httpContext.User.IsInRole(role))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool IsOwner(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            IPrincipal user = httpContext.User;

            if (httpContext.Request == null)
            {
                throw new ArgumentNullException("HttpContext.Request");
            }

            if (httpContext.Request.RequestContext == null || httpContext.Request.RequestContext.RouteData == null || httpContext.Request.RequestContext.RouteData.Values == null)
            {
                return false;
            }

            var generatorRouteValue = httpContext.Request.RequestContext.RouteData.Values["id"];
            Int32 generatorId;

            if (generatorRouteValue == null)
            {
                generatorRouteValue = httpContext.Request.Form["id"];
            }
            
            if (generatorRouteValue == null || !Int32.TryParse((string)generatorRouteValue, out generatorId))
            {
                return false;
            }

            return DataAccess.DataContext.User.OwnerOfGenerator.Contains(generatorId);
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}