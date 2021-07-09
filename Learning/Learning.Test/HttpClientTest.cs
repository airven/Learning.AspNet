using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Test
{
    public class HttpClientTest : ITask
    {
        //private static HttpClient _httpClient;
        //private static HttpClient Client
        //{
        //    get { return _httpClient; }
        //}

        public void Print()
        {
            Console.WriteLine(PostV4());
            Console.WriteLine("Hello World!");
        }

        public async Task<HttpResponseMessage> GetV1Async()
        {
            HttpResponseMessage response;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:50049");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.Timeout = new TimeSpan(0, 1, 0);
                    //httpClient.DefaultRequestHeaders.Add("Authorization", "EndpointKey " + endpoint_key);
                    using (response = await httpClient.GetAsync("api/getv2").ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Innal Error"
                };
            }
            return response;
        }

        /// <summary>
        /// PostAsync by FormUrlEncodedContent
        /// </summary>
        /// <returns>Task<HttpResponseMessage></returns>
        public async Task<HttpResponseMessage> PostV1()
        {
            HttpResponseMessage response;
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("LastName", "abcd");
                parameters.Add("FirstName", "FirstName");
                parameters.Add("City", "suzhou");

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:50049");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.Timeout = new TimeSpan(0, 1, 0);
                    using (response = await httpClient.PostAsync("api/PostV3", new FormUrlEncodedContent(parameters)))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Innal Error"
                };
            }
            return response;
        }

        /// <summary>
        /// PostAsync by Json Request
        /// </summary>
        /// <returns>Task<HttpResponseMessage></returns>
        public async Task<HttpResponseMessage> PostV2()
        {
            HttpResponseMessage response;
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("LastName", "abcd");
                parameters.Add("FirstName", "FirstName");
                parameters.Add("City", "suzhou");

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:50049");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.Timeout = new TimeSpan(0, 1, 0);

                    //var httpCotent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                    MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                    // Use the JSON formatter to create the content of the request body.
                    HttpContent httpCotent = new ObjectContent<object>(parameters, jsonFormatter);
                    using (response = await httpClient.PostAsync("api/PostV3", httpCotent).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Innal Error"
                };
            }
            return response;
        }

        /// <summary>
        /// PostAsync by sendAsync
        /// </summary>
        /// <returns>Task<HttpResponseMessage></returns>
        public async Task<HttpResponseMessage> PostV3()
        {
            HttpResponseMessage response;
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("LastName", "abcd");
                parameters.Add("FirstName", "FirstName");
                parameters.Add("City", "suzhou");

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:50049");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var httpContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/PostV3")
                    {
                        Content = httpContent,
                    };
                    using (response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Innal Error"
                };
            }
            return response;
        }

        public async Task<HttpResponseMessage> PostV4()
        {
            HttpResponseMessage response;
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("LastName", "abcd");
                parameters.Add("FirstName", "FirstName");
                parameters.Add("City", "suzhou");

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:50049");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var httpContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                    HttpRequestMessage requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("api/PostV3"),
                        Content = httpContent,
                    };
                    using (response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Innal Error"
                };
            }
            return response;
        }

        public async Task<HttpResponseMessage> ResponseTestV1()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Hello!")
            };
            // Note: TaskCompletionSource creates a task that does not contain a delegate. 
            // 注：TaskCompletionSource创建一个不含委托的任务。
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            // Also sets the task state to "RanToCompletion"                    
            // 也将此任务设置成“RanToCompletion（已完成）”
            return await tsc.Task;
        }

        public async Task<HttpResponseMessage> ResponseTestV2()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No product with ID = {0}", 1)),
                ReasonPhrase = "Product ID Not Found"
            };
            return await Task.FromResult(resp);
        }

        private async Task<T> GetRequest<T>(string uri) where T:class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<T>(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }

        private async Task<TOut> PostRequest<TIn, TOut>(string uri, TIn content)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    var serialized = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await client.PostAsync(uri, serialized))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }

    }
}
