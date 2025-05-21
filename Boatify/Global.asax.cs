using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ChuyenDI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported && Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                try
                {
                    var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    
                    if (ticket != null && !ticket.Expired)
                    {
                        var identity = new System.Security.Principal.GenericIdentity(ticket.Name);
                        Context.User = new System.Security.Principal.GenericPrincipal(identity, null);
                    }
                }
                catch { /* Ignore any exception and proceed with default authentication */ }
            }
        }
    }
}
