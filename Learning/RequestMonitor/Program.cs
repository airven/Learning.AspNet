using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RequestMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureLogging(builder =>
                     builder.AddConsole(c => { c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]";}));
                    if(args.Length>0)
                    {
                        webBuilder.UseUrls($"http://localhost:{args[0]}", $"https://localhost:{int.Parse(args[0]) + 1}");
                    }
                });

    }
}
