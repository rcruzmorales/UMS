using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using UMS.Models;
using Excel = OfficeOpenXml;

namespace UMS.Controllers
{
    public class AdministrationController : Controller
    {
        private UMSDBEntities db = new UMSDBEntities();
        private List<Tuple<DbEntityValidationException,int>> DBEVEL = new List<Tuple<DbEntityValidationException,int>>();
        private List<int> row = new List<int>();


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

        //admin default page
        [NoDirectAccess]
        public ActionResult Index()
        {
            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
            return View(users.ToList());
        }

        // User detailed information from admin page
        [NoDirectAccess]
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

        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.Picture_Id = new SelectList(db.ProfilePictures, "Picture_Id", "Filename");
            ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName");
            ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName");
            ViewBag.User_Id = new SelectList(db.UserCredentials, "User_Id", "Password");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create(UserInfo user)
        {
            //user.FirstName = HtmlandScriptCheck(user.FirstName);
            //user.LastName = HtmlandScriptCheck(user.LastName);
            //user.UserName = HtmlandScriptCheck(user.UserName);
            //user.Address = HtmlandScriptCheck(user.Address);
            //user.City = HtmlandScriptCheck(user.City);
            //user.Country = HtmlandScriptCheck(user.Country);

            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);

            try
            {

                var verifyEmail = db.UserInfoes.Where(e => e.Email == user.Email).FirstOrDefault();
                var verifyUsername = db.UserInfoes.Where(u => u.UserName == user.UserName).FirstOrDefault();

                if (verifyEmail != null || verifyUsername != null)
                {
                    ViewBag.Duplicate = "1";

                    return View("Index", users.ToList());
                }
                if (ModelState.IsValid)
                {
                    user.Picture_Id = 1;
                    db.UserInfoes.Add(user);
                    db.SaveChanges();

                    db.ProfilePictures.Add(new ProfilePicture { User_Id = user.User_Id, Filename = "user_icon.png", Type = "image/png", Size = 10, Data = "C:\\Users\\rcruzmorales\\Desktop\\.NET Projects\\UMS (User Management System)\\UMS\\UMS\\UMS\\Content\\Images\\user_icon.png" });
                    db.SaveChanges();

                    string password = GeneratePassword(12, 6);
                    db.UserCredentials.Add(new UserCredential { User_Id = user.User_Id, Password = password });
                    db.SaveChanges();
                    ViewBag.ConfirmAdd = 1;
                    users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
                    return View("Index", users.ToList());
                }

            }
            catch (Exception)
            {
                ViewBag.ConfirmAdd = 0;
                ViewBag.Picture_Id = new SelectList(db.ProfilePictures, "Picture_Id", "Filename", user.Picture_Id);
                ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName", user.Role_Id);
                ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName", user.Status_Id);
                ViewBag.User_Id = new SelectList(db.UserCredentials, "User_Id", "Password", user.User_Id);


                return View("Index", users.ToList());
            }

            ViewBag.ConfirmAdd = 0;
            ViewBag.Picture_Id = new SelectList(db.ProfilePictures, "Picture_Id", "Filename", user.Picture_Id);
            ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName", user.Role_Id);
            ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName", user.Status_Id);

            return View("Index", users.ToList());
        }

        //private string HtmlandScriptCheck(string field)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(field);
        //    sb.Replace("<script>", "<nice>");
        //    sb.Replace("</script>", "</try>");
        //    sb.Replace("on", "no");

        //    return sb.ToString();
        //}

        // Used to generate a random password when creating a user from admin page
        private string GeneratePassword(int length, int specialChar)
        {
            return Membership.GeneratePassword(length, specialChar);
        }

        //Edit user modal form admin page
        [NoDirectAccess]
        [HttpGet]
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
            ViewBag.ConfirmEdit = 0;
            ViewBag.Picture_Id = new SelectList(db.ProfilePictures, "Picture_Id", "Filename", userInfo.Picture_Id);
            ViewBag.Role_Id = new SelectList(db.Roles, "Role_Id", "RoleName", userInfo.Role_Id);
            ViewBag.Status_Id = new SelectList(db.Status, "Status_Id", "StatusName", userInfo.Status_Id);
            return View(userInfo);
        }

        //Edit user inforamtion to database from admin page
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                return View("Index", users.ToList());
            }

            return View("Index", users.ToList());
        }

        // Begin session from admin page
        public void SignIn(int id, string firstName, string email, string address, string city, string state, string zip, string fileName)
        {
            Session["FileName"] = fileName;
            Session["Id"] = id;
            Session["FirstName"] = firstName;
            Session["Email"] = email;
            Session["Address"] = address;
            Session["City"] = city;
            Session["State"] = state;
            Session["Zip"] = zip;
        }

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

        // Remove user message to confirm delete 
        public ActionResult Delete(int? id)
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


        //Removes user from database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            UserCredential credential = db.UserCredentials.Find(id);
            db.UserCredentials.Remove(credential);
            db.SaveChanges();

            UserInfo user = db.UserInfoes.Find(id);
            db.UserInfoes.Remove(user);
            db.SaveChanges();

            ProfilePicture picture = db.ProfilePictures.First(p => p.User_Id == id);
            db.ProfilePictures.Remove(picture);
            db.SaveChanges();
            ViewBag.ConfirmRemove = 1;

            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
            return View("Index", users.ToList());
        }


        // Batch upload,edit from excel file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelImport(FormCollection info)
        {
            var users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);

            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];

                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        
                        var usersList = new List<UserInfo>();

                        using (var package = new Excel.ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                var user = new UserInfo();
                                user.UserName = workSheet.Cells[rowIterator, 1].Value.ToString();
                                user.Email = workSheet.Cells[rowIterator, 2].Value.ToString();
                                user.FirstName = workSheet.Cells[rowIterator, 3].Value.ToString();
                                user.LastName = workSheet.Cells[rowIterator, 4].Value.ToString();
                                user.PhoneNumber = workSheet.Cells[rowIterator, 5].Value.ToString();
                                user.Address = workSheet.Cells[rowIterator, 6].Value.ToString();
                                user.City = workSheet.Cells[rowIterator, 7].Value.ToString();
                                user.Country = workSheet.Cells[rowIterator, 8].Value.ToString();
                                user.Zipcode = workSheet.Cells[rowIterator, 9].Value.ToString();

                                if (workSheet.Cells[rowIterator, 10].Value.ToString() == "Admin" || workSheet.Cells[rowIterator, 10].Value.ToString() == "1")
                                {
                                    user.Role_Id = 1;
                                }

                                else if (workSheet.Cells[rowIterator, 10].Value.ToString() == "User" || workSheet.Cells[rowIterator, 10].Value.ToString() == "2")
                                {
                                    user.Role_Id = 2;
                                }

                                if (workSheet.Cells[rowIterator, 11].Value.ToString() == "Active" || workSheet.Cells[rowIterator, 11].Value.ToString() == "1")
                                {
                                    user.Status_Id = 1;
                                }

                                else if (workSheet.Cells[rowIterator, 11].Value.ToString() == "Inactive" || workSheet.Cells[rowIterator, 11].Value.ToString() == "2")
                                {
                                    user.Status_Id = 2;
                                }

                                usersList.Add(user);
                            }

                            if (verifyUsers(usersList))
                            {
                                ViewBag.ConfirmUpload = 1;
                                users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role)
                                    .Include(u => u.Status).Include(u => u.UserCredential);
                                return View("Index", users.ToList());
                            }

                            else
                            {
                                ViewBag.ConfirmUpload = 0;
                                users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role)
                                    .Include(u => u.Status).Include(u => u.UserCredential);
                                
                                return View("Index", users.ToList());

                            }
                        }
                    }
                }

                catch (Exception)
                {
                    ViewBag.ConfirmUpload = 1;
                    ViewBag.ExcelFormat = 1;

                }
            }
            users = db.UserInfoes.Include(u => u.ProfilePicture).Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
            return View("Index", users.ToList());

        }

        // Verify user from excel file validations to insert or update user information
        private bool verifyUsers(List<UserInfo> userList)
        {
            var count = 2;
            
            foreach (var user in userList)
            {
                var verifyUser = db.UserInfoes.Where(u => u.UserName == user.UserName).FirstOrDefault();
                var verifyEmail = db.UserInfoes.Where(e => e.Email == user.Email).FirstOrDefault();
                try
                {

                    if (verifyUser != null && verifyEmail != null)
                    {

                        verifyUser.FirstName = user.FirstName;
                        verifyUser.LastName = user.LastName;
                        verifyUser.PhoneNumber = user.PhoneNumber;
                        verifyUser.Address = user.Address;
                        verifyUser.City = user.City;
                        verifyUser.Country = user.Country;
                        verifyUser.Zipcode = user.Zipcode;
                        verifyUser.Role_Id = user.Role_Id;
                        verifyUser.Status_Id = user.Status_Id;
                        db.Entry(verifyUser).State = EntityState.Modified;
                        db.SaveChanges();
                        
                    }

                    else if ((verifyUser == null && verifyEmail != null) || (verifyUser != null && verifyEmail == null))
                    {

                    }

                    else
                    {
                        user.Picture_Id = 1;
                        db.UserInfoes.Add(user);
                        db.SaveChanges();

                        db.ProfilePictures.Add(new ProfilePicture
                        {
                            User_Id = user.User_Id,
                            Filename = "user_icon.png",
                            Type = "image/png",
                            Size = 10,
                            Data = "C:\\Users\\rcruzmorales\\Desktop\\.NET Projects\\" +
                                   "UMS (User Management System)\\" +
                                   "UMS\\UMS\\UMS\\Content\\Images\\user_icon.png"
                        });
                        db.SaveChanges();

                        string password = GeneratePassword(12, 6);
                        db.UserCredentials.Add(new UserCredential {User_Id = user.User_Id, Password = password});
                        db.SaveChanges();
                        

                    }
                }
                catch (DbEntityValidationException ve)
                {
                    
                    DBEVEL.Add(new Tuple<DbEntityValidationException, int>(ve,count));
                    
                    db.Entry(verifyUser).State = EntityState.Unchanged;
                }

                ++count;
            }



            return true;
        }

        //return a list of validation errors
        public List<Tuple<DbEntityValidationException,int>> getErrorList()
        {
            return DBEVEL;
        }

        // return a list of row number where the errors are located in the excel file
        public List<int> getRows()
        {
            return row;
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
