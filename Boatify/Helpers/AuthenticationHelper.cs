using System;
using System.Web;
using System.Web.Security;

namespace ChuyenDI.Helpers
{
    public static class AuthenticationHelper
    {        public static void SetAuthCookie(string username, bool rememberMe)
        {
            // Create FormsAuthenticationTicket
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,                              // Version
                username,                       // User name
                DateTime.Now,                   // Issue time
                DateTime.Now.AddMinutes(rememberMe ? 2880 : 30), // Expiration (30 min or 48 hours)
                rememberMe,                     // Persistent
                username,                       // User data
                FormsAuthentication.FormsCookiePath);  // Cookie path

            // Encrypt the ticket
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

            // Set cookie properties
            if (rememberMe)
                authCookie.Expires = ticket.Expiration;
                
            // Ensure cookie domain and path are set correctly
            authCookie.Path = FormsAuthentication.FormsCookiePath;
              // Make sure SameSite attribute is set appropriately for modern browsers if .NET Framework supports it
            // In older frameworks this property might not exist
            try
            {
                // Setting SameSite to "Lax" through reflection for compatibility with different .NET versions
                var sameSiteProperty = typeof(HttpCookie).GetProperty("SameSite");
                if (sameSiteProperty != null)
                {
                    sameSiteProperty.SetValue(authCookie, 1, null); // 1 = Lax in SameSiteMode enum
                }
            }
            catch
            {
                // Ignore if not supported in this .NET version
            }
            
            // Clear any existing auth cookie first
            if (HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            }

            // Add the cookie to the response
            HttpContext.Current.Response.Cookies.Add(authCookie);
            
            // Also call FormsAuthentication.SetAuthCookie as a backup method
            FormsAuthentication.SetAuthCookie(username, rememberMe);
        }        public static void SignOut()
        {
            // Clear authentication cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            authCookie.Expires = DateTime.Now.AddDays(-1);
            authCookie.Path = FormsAuthentication.FormsCookiePath;
            authCookie.Value = string.Empty;
            
            // Ensure we're clearing the correct cookie with proper path
            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }

            // Call FormsAuthentication.SignOut for good measure
            FormsAuthentication.SignOut();
            
            // Also attempt to clear session state
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        public static bool IsAuthenticated()
        {
            return HttpContext.Current.User != null &&
                   HttpContext.Current.User.Identity != null &&
                   HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }
}