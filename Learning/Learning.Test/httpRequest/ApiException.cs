﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Test.httpRequest
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }
}
