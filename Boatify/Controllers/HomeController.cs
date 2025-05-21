using System;
using System.Web.Mvc;

namespace ChuyenDI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About ChuyenDI";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";
            return View();
        }
        public ActionResult TestAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "You are authenticated as: " + User.Identity.Name;
            }
            else
            {
                ViewBag.Message = "You are NOT authenticated";
            }

            return View();
        }
    }

}