using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using UMS.Models;
using Microsoft.Security.Application;
using Microsoft.Security.Application.TextConverters.HTML;

namespace UMS.Controllers
{
    public class UserController : Controller
    {

        private UMSDBEntities db = new UMSDBEntities();

        //Used to prevent going back to the pages and go to the login page if user is not logged in
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                   RouteValueDictionary(new { controller = "User", action = "Login", area = "" }));
                }
            }
        }
        // Create View
        [NoDirectAccess]
        public ActionResult Create()
        {

            ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName");
            ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName");

            return View();
        }

        //Create User in Register Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(UserInfo user)
        {
            ViewBag.Duplicate = "0";


            try
            {
                var verifyEmail = db.UserInfoes.Where(e => e.Email == user.Email).FirstOrDefault();
                var verifyUsername = db.UserInfoes.Where(u => u.UserName == user.UserName).FirstOrDefault();
                if (verifyEmail != null || verifyUsername != null)
                {
                    ViewBag.Duplicate = "1";
                    return View("Login");
                }
                if (ModelState.IsValid)
                {

                    user.Picture_Id = 1;
                    db.UserInfoes.Add(user);
                    db.SaveChanges();

                    db.ProfilePictures.Add(new ProfilePicture
                    {
                        User_Id = user.User_Id,
                        Filename = "user_icon.png",
                        Type = "image/png", Size = 10,
                        Data = "C:\\Users\\rcruzmorales\\Desktop\\.NET Projects\\" +
                               "UMS (User Management System)\\" +
                               "UMS\\UMS\\UMS\\Content\\Images\\user_icon.png"
                    });
                    db.SaveChanges();

                    string password = GeneratePassword(12, 6);
                    db.UserCredentials.Add(new UserCredential { User_Id = user.User_Id, Password = password });
                    db.SaveChanges();

                    ViewBag.Password = user.UserCredential.Password;
                    return View("Login");
                }

                ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName", user.Role_Id);
                ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName", user.Status_Id);

                return View("LogIn", user);
            }
            catch (DbEntityValidationException error)
            {
                ViewBag.Error = error.Data.ToString();

            }
            ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName", user.Role_Id);
            ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName", user.Status_Id);

            return View("LogIn", user);
        }


        //private string HtmlandScriptCheck(string field)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(field);
        //    sb.Replace("<script>", "<nice>");
        //    sb.Replace("</script>", "</try>");
           
        //    return sb.ToString();
        //}

        // Used to generate a random password when new user is created or user forgot password

        // Used to generate a random password when user forgets password or when a user is registering
        private string GeneratePassword(int length, int specialChar)
        {
           
            return Membership.GeneratePassword(length, specialChar);
        }
    

        //User Login view
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                if (Session["Role"].ToString() == "User")
                {

                    RedirectToAction("Index", "Client");
                }
                else
                {
                    RedirectToAction("Index", "Administration");
                }
            }
            AbandonSession();

            return View();
        }

        //User Login Process
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Login(UserInfo user)
        {
            try
            {
                var verifyUser = db.UserInfoes.Where(u => u.Email.Equals(user.Email))
                                 .Where(p => p.UserCredential.Password.Equals(user.UserCredential.Password))
                                 .FirstOrDefault();

                if (verifyUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Authentication failed.");

                    return View();
                }


                if (verifyUser.Role_Id == 1)
                {
                    Session["Role"] = "Admin";


                    if (verifyUser.Status_Id == 2)
                    {
                        ViewBag.Status = verifyUser.Status_Id;
                        return View();
                    }

                    else
                    {
                        SignIn(verifyUser.User_Id,
                            verifyUser.FirstName,
                            verifyUser.Email,
                            verifyUser.Address,
                            verifyUser.City,
                            verifyUser.Country,
                            verifyUser.Zipcode,
                            verifyUser.ProfilePicture.Filename);

                        return RedirectToAction("Index", "Administration");
                    }

                }

                else if (verifyUser.Role_Id == 2)
                {
                    Session["Role"] = "User";


                    if (verifyUser.Status_Id == 2)
                    {
                        ViewBag.Status = verifyUser.Status_Id;
                        return View();
                    }

                    else
                    {
                        SignIn(verifyUser.User_Id,
                            verifyUser.FirstName, 
                            verifyUser.Email, 
                            verifyUser.Address, 
                            verifyUser.City, 
                            verifyUser.Country, 
                            verifyUser.Zipcode,
                            verifyUser.ProfilePicture.Filename);

                        return RedirectToAction("Index", "Client");
                    }
                }

                else
                {
                    return HttpNotFound();
                }
            }

            catch (Exception err)
            {
                ViewBag.Error = "ERROR";

                ViewBag.ErrorMessage = err.Message;

                return View();
            }

        }

        // Abandon Session
        public ActionResult Logoff()
        {

            AbandonSession();
            return RedirectToAction("Login");
        }

        //Set the user session when logged in
        public void SignIn(int id,string firstName, string email, string address, string city, string state, string zip,string filename)
        {
            Session["FileName"] = filename;
            Session["Id"] = id;
            Session["FirstName"] = firstName;
            Session["Email"] = email;
            Session["Address"] = address;
            Session["City"] = city;
            Session["State"] = state;
            Session["Zip"] = zip;
        }

        // Forgot passoword view
        public ActionResult ForgotPassword()
        {
            return View();
        }


        //Verify Email to generate a new random password
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ForgotPassword(UserInfo user)
        {

            var verifyEmail = db.UserInfoes.Where(e => e.Email.Equals(user.Email)).FirstOrDefault();

            if (verifyEmail == null)
            {
                ViewBag.ErrorMessage = "Invalid Credentials";
                return View();
            }

            var newPass = Membership.GeneratePassword(12, 6);


            var verifyPass = db.UserCredentials.Where(p => p.User_Id.Equals(verifyEmail.User_Id)).FirstOrDefault();
            verifyPass.Password = newPass;
            db.Entry(verifyPass).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Password = verifyPass.Password;
            return View();
        }

        // This method is used when user logs off 
        public ActionResult AbandonSession()

        {

            Session.Clear();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.Cache.VaryByParams.IgnoreParams = true;

            Response.Cache.SetNoStore();

            Session.Abandon();

            return RedirectToAction("Login", "User");

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

