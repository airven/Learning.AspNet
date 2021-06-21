using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.SessionState;
using System.Web.Http;

namespace Learning.AspNetMvc.Controllers
{
    [RoutePrefix("Mytest")]
    public class PartController : ApiController
    {
        public const string SERVIRABLE = "SERVIRABLE";
        [HttpGet, Route("Get")]
        public HttpResponseMessage Get()
        {
            var studentList = new List<student> {
                new student{ id="a",sname="abc"},
                 new student{ id="b",sname="abcd"},
                  new student{ id="c",sname="abcde"},
            };
            if (HttpContext.Current.Session == null || HttpContext.Current.Session[SERVIRABLE] == null)
                HttpContext.Current.Session[SERVIRABLE] = studentList;
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("Test")]
        public IEnumerable<student> Test()
        {
            var stulist = HttpContext.Current.Session[SERVIRABLE] as List<student>;
            return stulist;
        }
    }
    public class student
    {
        public string id { get; set; }
        public string sname { get; set; }
    }
}
