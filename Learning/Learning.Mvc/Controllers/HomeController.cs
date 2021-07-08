using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Learning.Mvc.Models;

namespace Learning.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();


        public HomeController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                string FirstName = Request.Form["FirstName"];
                string LastName = Request.Form["LastName"];
                string Address = Request.Form["Address"];
                string City = Request.Form["City"];
                string Country = Request.Form["Country"];
                string Title = Request.Form["Title"];
                string Notes = Request.Form["Notes"];

                if (!IsEmailExist(FirstName))
                {
                    if (Request.Files.Count > 0)
                    {
                        try
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            string fname = file.FileName;
                            fname = Path.Combine(Server.MapPath("~/Images/"), fname);
                            file.SaveAs(fname);
                            Employee userobj = new Employee
                            {
                                FirstName = FirstName,
                                LastName = LastName,
                                Address = Address,
                                City = City,
                                Country = Country,
                                Title = Title,
                                Notes = Notes,
                                PhotoPath = "/Images/" + file.FileName
                            };
                            db.Employees.Add(userobj);
                            if (db.SaveChanges() > 0)
                            {
                                return Json("User registered Successfully!");
                            }
                            else
                            {
                                return Json("Something went wrong please try again later!");
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(ex.Message);
                        }
                    }
                    else
                    {
                        return Json("No files selected.");
                    }
                }
                else
                {
                    return Json("email already exist please try with diffrent one!");
                }

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return Json(null);
            }
        }

        private bool IsEmailExist(string firtName)
        {
            bool IsEmailExist = false;
            int count = db.Employees.Where(a => a.FirstName == firtName).Count();
            if (count > 0)
            {
                IsEmailExist = true;
            }
            return IsEmailExist;
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
