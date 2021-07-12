using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Learning.Test.webRequest
{
    public class WebRequestTest : ITask
    {
        public void Print()
        {
            var result = GetAccessToken();
            Console.WriteLine(result);
        }

        public string GetAccessToken()
        {  //GET获取accessToken的参数 corid
            string uri = "你的URL";
            //创建请求
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //WebClient是对HttpWebRequest的抽象,WebClient使用简单，但速度慢;Restsharp兼具WebClient和HttpWebClient的优点；HttpClient是.NetCore中的概念，更适合异步编程
            WebRequest request = WebRequest.Create(uri);
            //请求设置
            request.Credentials = CredentialCache.DefaultCredentials;
            //创建应答接收
            WebResponse response = request.GetResponse();
            //创建应答读写流
            string accessToken;
            using (Stream streamResponse = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(streamResponse);
                string responseFromServer = reader.ReadToEnd();
                JObject res = (JObject)JsonConvert.DeserializeObject(responseFromServer);
                accessToken = res["access_token"].ToString();
                reader.Close();
            }
            //获得许可证凭证
            PostMail(accessToken);
            //关闭响应
            response.Close();
            return "success";
        }

        public void PostMail(string accessToken)
        {    //POST的API
            string uri = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken;
            //创建请求
            WebRequest myWebRequest = WebRequest.Create(uri);
            //请求设置
            myWebRequest.Credentials = CredentialCache.DefaultCredentials;
            myWebRequest.ContentType = "application/json;charset=UTF-8";
            myWebRequest.Method = "POST";
            //向服务器发送的内容
            using (Stream streamResponse = myWebRequest.GetRequestStream())
            {
                //创建JSON格式的发送内容
                JObject postedJObject = new JObject
              {
                  //在此处设置发送内容及对象
                  { "touser", "Heavy" },
                  { "msgtype", "text" },
                  { "agentid", 1000002 }
              };
                JObject text = new JObject
              {
                  {"content","内容来自网站--内容可自行编辑--heavy"}
              };
                postedJObject.Add("text", text);
                postedJObject.Add("safe", 0);
                //将传送内容编码
                String paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
                byte[] byteArray = Encoding.UTF8.GetBytes(paramString);
                //向请求中写入内容
                streamResponse.Write(byteArray, 0, byteArray.Length);
            }
            //创建应答
            WebResponse myWebResponse = myWebRequest.GetResponse();
            //创建应答的读写流
            string responseFromServer;
            using (Stream streamResponse = myWebResponse.GetResponseStream())
            {
                StreamReader streamRead = new StreamReader(streamResponse);
                responseFromServer = streamRead.ReadToEnd();
            }
            //关闭应答
            myWebResponse.Close();
        }


        public async Task<string> GetAccessTokenAsync()
        {
            string uri = "你的URL";
            HttpClientHandler handler = new HttpClientHandler
            {
                //设置是否发送凭证信息，有的服务器需要验证身份，不是所有服务器需要
                UseDefaultCredentials = false

            };
            HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string accessToken;
            //回复结果直接读成字符串
            string resp = await response.Content.ReadAsStringAsync();
            JObject json = (JObject)JsonConvert.DeserializeObject(resp);
            accessToken = json["access_token"].ToString();
            //采用流读数据
            //using (Stream streamResponse = await response.Content.ReadAsStreamAsync())
            //{
            //    StreamReader reader = new StreamReader(streamResponse);
            //    string responseFromServer = reader.ReadToEnd();
            //    JObject res = (JObject)JsonConvert.DeserializeObject(responseFromServer);
            //    accessToken = res["access_token"].ToString();
            //    reader.Close();
            //}
            //获得许可证凭证
            PostMailAsync(accessToken);
            //关闭响应
            return "success";
        }

        public async void PostMailAsync(string accessToken)
        {
            string uri = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken;
            HttpClientHandler handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,

            };
            HttpClient httpClient = new HttpClient(handler);
            JObject postedJObject = new JObject
               {
                   //在此处设置发送内容及对象
                   { "touser", "Heavy" },
                   { "msgtype", "text" },
                   { "agentid", 1000002 }
               };
            JObject text = new JObject
               {
                   {"content","内容来自网站--内容可自行编辑--heavy"}
               };
            postedJObject.Add("text", text);
            postedJObject.Add("safe", 0);
            //将传送内容编码
            String paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
            //byte[] byteArray = Encoding.UTF8.GetBytes(paramString);
            HttpContent httpContent = new StringContent(paramString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, httpContent);
            //用来判断是否接收成功，否则抛出异常
            response.EnsureSuccessStatusCode();
        }

    }
}
