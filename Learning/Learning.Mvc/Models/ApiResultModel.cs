using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Learning.Mvc.Models
{
    public class ApiResultModel
    {
        public HttpStatusCode Status { get; set; }

        public object Data { get; set; }

        public string ErrorMessage { get; set; }
    }
}