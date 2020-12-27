using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.AspNetMvc.Areas.Account.Controllers
{
    public class HomeController : Controller
    {
        // GET: Account/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}