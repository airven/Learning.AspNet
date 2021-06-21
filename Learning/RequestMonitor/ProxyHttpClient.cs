using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestMonitor
{
    public class ProxyHttpClient
    {
        public HttpClient Client { get; private set; }
        public ProxyHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
        }
    }
}
