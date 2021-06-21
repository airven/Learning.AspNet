using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RequestMonitor.Controllers
{
    //[Route("api/[controller]")]
    [Route("/")]
    [ApiController]
    public class MonitorNewController : ControllerBase
    {

        private string _urls;
        private const string CDN_HEADER_NAME = "Cache-Control";
        private static readonly string[] NotForwardedHttpHeaders = new[] { "Connection", "Host" };
        private readonly ILogger<MonitorNewController> _logger;
        private readonly ProxyHttpClient httpClient;
        private readonly IUrlRewriter urlrewriter;

        public MonitorNewController(ILogger<MonitorNewController> logger, IConfiguration configuration, IUrlRewriter urlRewriter, ProxyHttpClient proxyHttpClient)
        {
            _logger = logger;
            _urls = configuration["ServerURL"];
            urlrewriter = urlRewriter;
            httpClient = proxyHttpClient;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            
            _logger.LogInformation("请求开始__________________________________________________");
            StringBuilder request_header = new StringBuilder();
            StringBuilder request_header1 = new StringBuilder();
            StringBuilder response_header = new StringBuilder();
            StringBuilder request_all = new StringBuilder();

            string request_body = "";

            foreach (var item in HttpContext.Request.Headers)
            {
                request_header.AppendLine(item.Key + ":" + item.Value);
            }
            _logger.LogInformation($"request_header:{request_header.ToString()}");

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                request_body = await reader.ReadToEndAsync();
            }
            _logger.LogInformation($"request_body_0:{request_body}");

            request_all.Append("request_header:" + request_header);
            //request_all.Append(request_body);
            _logger.LogInformation($"开始转发至{_urls},请稍候......");

            var context = Request.HttpContext;
            var targetUri = await urlrewriter.RewriteUri(context);
            if (targetUri != null)
            {
                using (var requestMessage = GenerateProxifiedRequest(context, targetUri))
                using (var response = await SendAsync(context, requestMessage, httpClient).ConfigureAwait(false))
                {
                    request_body = await response.Content.ReadAsStringAsync();
                    foreach(var requestNew in requestMessage.Headers)
                    {
                        request_header1.AppendLine(requestNew.Key + ":" +string.Join(',', requestNew.Value));
                    }
                    foreach (var head in context.Response.Headers)
                    {
                        response_header.AppendLine(head.Key + ":" + head.Value.ToString());
                    }

                    _logger.LogInformation($"request_header1:{request_header1.ToString()}");
                    _logger.LogInformation($"response_header:{response_header.ToString()}");
                    _logger.LogInformation($"response_body:{request_body}");

                    request_all.AppendLine("—————————————————————————————————————————————————————" );
                    request_all.AppendLine("request_header1:" + request_header1.ToString());
                    request_all.AppendLine("response_header:" + response_header.ToString());
                    request_all.AppendLine("response_body:" + request_body.ToString());
                }
            }


            _logger.LogInformation($"转发至{_urls}已结束......");
            _logger.LogInformation("请求结束__________________________________________________");
            return Ok(request_all.ToString());
        }

        private async Task<HttpResponseMessage> SendAsync(HttpContext context, HttpRequestMessage requestMessage, ProxyHttpClient proxyHttpClient)
        {
            var responseMessage = await proxyHttpClient.Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
            context.Response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            //foreach (var header in responseMessage.Content.Headers)
            //{
            //    context.Response.Headers[header.Key] = header.Value.ToArray();
            //}
            context.Response.Headers.Remove("transfer-encoding");

            if (!context.Response.Headers.ContainsKey(CDN_HEADER_NAME))
            {
                context.Response.Headers.Add(CDN_HEADER_NAME, "no-cache, no-store");
            }
            //await responseMessage.Content.CopyToAsync(context.Response.Body);
            return responseMessage;
        }

        private static HttpRequestMessage GenerateProxifiedRequest(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyRequestContentAndHeaders(context, requestMessage);
            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = context.Request.Host.Value;
            requestMessage.Method = GetMethod(context.Request.Method);
            return requestMessage;
        }

        private static void CopyRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                if (!NotForwardedHttpHeaders.Contains(header.Key))
                {
                    if (header.Key != "User-Agent")
                    {
                        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                        {
                            requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                        }
                    }
                    else
                    {
                        string userAgent = header.Value.Count > 0 ? (header.Value[0] + " " + context.TraceIdentifier) : string.Empty;

                        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, userAgent) && requestMessage.Content != null)
                        {
                            requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, userAgent);
                        }
                    }
                }
            }
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }
    }
}
