//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RequestMonitor.Controllers
//{
//    [ApiController]
//    [Route("/")]
//    public class MonitorController : ControllerBase
//    {
//        private readonly ILogger<MonitorController> _logger;
//        public MonitorController(ILogger<MonitorController> logger)
//        {
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<ActionResult> Get()
//        {
//            _logger.LogInformation("请求开始__________________________________________________");
//            StringBuilder request_header = new StringBuilder();
//            StringBuilder request_all = new StringBuilder();

//            string request_body = "";

//            foreach (var item in HttpContext.Request.Headers)
//            {
//                request_header.AppendLine(item.Key + ":" + item.Value);
//            }

//            _logger.LogInformation($"request_header:{request_header.ToString()}");

//            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
//            {
//                request_body = await reader.ReadToEndAsync();
//            }
//            _logger.LogInformation($"request_body:{request_body}");

//            request_all.Append(request_header);
//            request_all.AppendLine(request_body);

//            _logger.LogInformation("请求结束__________________________________________________");
//            return Ok(request_all.ToString());
//        }
//    }
//}
