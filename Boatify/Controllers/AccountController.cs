using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using ChuyenDI.Helpers;
using ChuyenDI.Models;
using ChuyenDI.Models.ViewModels;

namespace ChuyenDI.Controllers
{
    public class AccountController : Controller
    {
        private ChuyenDIDbContext db = new ChuyenDIDbContext();

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Clear any existing authentication first
                    AuthenticationHelper.SignOut();
                    
                    // Check if user exists
                    var user = db.Users.FirstOrDefault(u => u.Username == model.Username);

                    if (user != null)
                    {
                        // Verify password
                        bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                        
                        if (passwordValid)
                        {
                            // Set authentication cookie
                            AuthenticationHelper.SetAuthCookie(user.Username, model.RememberMe);

                            // Check if user is now authenticated
                            if (User.Identity.IsAuthenticated)
                            {
                                // Success - redirect appropriately
                                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                // Authentication didn't work properly
                                ModelState.AddModelError("", "Login successful but authentication failed. Please try again.");
                            }
                        }
                        else
                        {
                            // Invalid password
                            ModelState.AddModelError("Password", "The password is incorrect.");
                        }
                    }
                    else
                    {
                        // User not found
                        ModelState.AddModelError("Username", "Username not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app)
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationHelper.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }

                // Check if email already exists
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();

                AuthenticationHelper.SetAuthCookie(user.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: Account/Profile
        [Authorize]
        public ActionResult Profile()
        {
            string username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/Edit
        [Authorize]
        public ActionResult Edit()
        {
            string username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;
                var user = db.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                // Only update email if it changed and is not already used
                if (user.Email != model.Email)
                {
                    if (db.Users.Any(u => u.Email == model.Email && u.UserId != user.UserId))
                    {
                        ModelState.AddModelError("Email", "Email is already registered.");
                        return View(model);
                    }

                    user.Email = model.Email;
                }

                db.SaveChanges();
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}