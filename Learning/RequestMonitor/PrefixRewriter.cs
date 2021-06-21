using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestMonitor
{
    public class PrefixRewriter:IUrlRewriter
    {
        //前缀值
        private readonly PathString _prefix;
        //转发的地址
        private readonly string _newHost; 

        public PrefixRewriter(PathString prefix, string newHost)
        {
            _prefix = prefix;
            _newHost = newHost;
        }

        public Task<Uri> RewriteUri(HttpContext context)
        {
            //判断访问是否含有前缀
            if (context.Request.Path.StartsWithSegments(_prefix))
            {
                var newUri = context.Request.Path.Value.Remove(0, _prefix.Value.Length) + context.Request.QueryString;
                var targetUri = new Uri(_newHost + newUri);
                return Task.FromResult(targetUri);
            }

            return Task.FromResult((Uri)null);
        }
    }
}
