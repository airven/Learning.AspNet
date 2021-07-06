using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;

using AspNetIdentityDemo.Models;
using AspNetIdentityDemo.Security;


namespace AspNetIdentityDemo.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            MyIdentityDbContext db = new MyIdentityDbContext();

            UserStore<MyIdentityUser> userStore = new UserStore<MyIdentityUser>(db);
            UserManager<MyIdentityUser> userManager = new UserManager<MyIdentityUser>(userStore);
            
            MyIdentityUser user = userManager.FindByName(HttpContext.User.Identity.Name);

            NorthwindEntities northwindDb = new NorthwindEntities();
            List<Customer> model = null;

            if(userManager.IsInRole(user.Id, "Administrator"))
            {
                model = northwindDb.Customers.ToList();
            }

            if(userManager.IsInRole(user.Id, "Operator"))
            {
                model = northwindDb.Customers.Where(c => c.Country == "USA").ToList();
            }

            ViewBag.FullName = user.FullName;

            return View(model);
        }
    }
}