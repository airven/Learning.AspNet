using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestMonitor
{
    public interface IUrlRewriter
    {
        Task<Uri> RewriteUri(HttpContext context);
    }
}
