using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Learning.Mvc.Models;
namespace Learning.Mvc.Controllers
{
    public class EmployeeController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        public EmployeeController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllEmployees()
        {
            try
            {
                var item = db.Employees.Select(x=>new { x.EmployeeID,x.FirstName,x.LastName,x.Address,x.City,x.Region}).ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(null);
            }
           
        }
        public ActionResult LoadEditEmployeesPopup(int EmployeesId)
        {
            try
            {
                var model = db.Employees.Where(a => a.EmployeeID == EmployeesId).FirstOrDefault();
                return PartialView("_UpdateEmployee", model);
            }
            catch (Exception ex)
            {
                return PartialView("_UpdateEmployee");
            }
        }
        public ActionResult LoadaddEmployeesPopup()
        {
            try
            {
                return PartialView("_AddEmployee");
            }
            catch (Exception ex)
            {
                return PartialView("_AddEmployee");
            }
        }

        public JsonResult AddEmployees(Employee employee)
        {
            string status = "success";
            try
            {
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateEmployees(Employee employee)
        {
            string status = "success";
            try
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return Json(employee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEmployees(int EmployeesId)
        {
            string status = "success";
            try
            {
                var pateint = db.Employees.Find(EmployeesId);
                db.Employees.Remove(pateint);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return Json(status, JsonRequestBehavior.AllowGet);
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