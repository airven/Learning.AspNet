
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
                string FullName = Request.Form["FullName"];
                string Email = Request.Form["Email"];
                string Password = Request.Form["Password"];
                if (!IsEmailExist(Email))
                {
                    if (Request.Files.Count > 0)            // Checking if  Request object  has file
                    {
                        try
                        {
                            using (db)
                            {
                                HttpPostedFileBase file = Request.Files[0];
                                string fname = file.FileName;
                                // Get the complete folder path and store the file inside it.  
                                fname = Path.Combine(Server.MapPath("~/Images/"), fname);
                                file.SaveAs(fname);
                                Employee userobj = new Employee
                                {
                                    //Email_Id = Email,
                                    //FullName = FullName,
                                    //PhotoPath = "/Images/" + file.FileName
                                };
                                db.Employees.Add(userobj);
                                if (db.SaveChanges() > 0)
                                {
                                    //Set MVC and Login Authentication
                                    return Json("User registered Successfully!");
                                }
                                else
                                {
                                    return Json("Something went wrong please try again later!");
                                }
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
            return View();
        }


        private bool IsEmailExist(string email)
        {
            bool IsEmailExist = false;
            using (db)
            {
                int count = db.Employees.Where(a => a.HomePhone == email).Count();
                if (count > 0)
                {
                    IsEmailExist = true;
                }
            }
            return IsEmailExist;
        }
    }
}
