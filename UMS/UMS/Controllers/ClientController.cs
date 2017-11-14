using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UMS.Models;
using Microsoft.AspNet.Identity;
using System.Web.Routing;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.Security.Application;
using System.Text;

namespace UMS.Controllers
{ 
    public class ClientController : Controller
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
          
        //Main View of the user
        [NoDirectAccess]
        public ActionResult Index()
        {
           
            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
          
            return View(users.ToList());
        }


        //User details information when using the view button in the table
        [NoDirectAccess]
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Write(".NET session has Expired");

                return RedirectToAction("Login", "User");


            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserInfo user = db.UserInfoes.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", user);
        }


        //User edit modal page to view
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserInfo userInfo = db.UserInfoes.Find(id);
            if (userInfo == null)
            {
                return HttpNotFound();
            }

            return View(userInfo);
        }
        

        //Edit user process to change and update user inforamtion in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(UserInfo user, HttpPostedFileBase file)
        {

            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);

            try
            {
                if (Session.Contents.Count == 0)
                {
                    Response.Write(".NET session has Expired");

                    return RedirectToAction("Login", "User");


                }

                if (file != null && file.ContentLength > 0)
                {

                    string path = Path.Combine(Server.MapPath("~/Content/Images"),
                        Path.GetFileName(file.FileName));

                    file.SaveAs(path);

                    var picture = db.ProfilePictures.First(model => model.User_Id == user.User_Id);

                    picture.Filename = Path.GetFileName(file.FileName);
                    picture.Type = file.ContentType;
                    picture.Size = file.ContentLength;
                    picture.Data = path;
                    db.Entry(picture).State = EntityState.Modified;
                    db.SaveChanges();

                    user.Picture_Id = picture.Picture_Id;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Refresh();
                    ViewBag.ConfirmEdit = 1;

                }

                else if (ModelState.IsValid)
                {
                   
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Refresh();
                    ViewBag.ConfirmEdit = 1;

                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Fields are Required");
                }
            }
            catch (Exception)
            {
                users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
                return View("Index", users.ToList());
            }
    
            return View("Index",users.ToList());

        }

        //private string HtmlandScriptCheck(string field)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(field);
        //    sb.Replace("<script>", "<nice>");
        //    sb.Replace("</script>", "</try>");
        //    sb.Replace("<on", "no");
        //    sb.Replace("<text", "nope");
        //    sb.Replace("<input", "what input");
        //    sb.Replace("<button", "what button");
        //    sb.Replace("</button>", "what button");
        //    sb.Replace("<href>", "noref");
           
        //    return sb.ToString();
        //}

        //User Session inforamtion
        public void SignIn(int id, string firstName, string email, string address, string city, string state, string zip, string filename)
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

        //When user edit inforamtion page loads and refresh the session
        public void Refresh()
        {
            var id = Convert.ToInt32(Session["Id"].ToString());
            var query = db.UserInfoes.Find(id);
            var pic = db.ProfilePictures.First(p => p.User_Id == query.User_Id);
            SignIn(query.User_Id,
                query.FirstName,
                query.Email,
                query.Address,
                query.City,
                query.Country,
                query.Zipcode,
                pic.Filename);
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
