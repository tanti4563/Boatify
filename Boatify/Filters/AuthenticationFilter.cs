using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace ChuyenDI.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // The user is not authenticated, redirect to login page
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "returnUrl", filterContext.HttpContext.Request.Url.PathAndQuery }
                    });
            }
            else
            {
                // The user is authenticated but not authorized, redirect to unauthorized page
                filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
            }
        }
    }
}