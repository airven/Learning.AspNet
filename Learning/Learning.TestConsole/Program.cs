using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Encrypt;

namespace Learning.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //var httpClient = new HttpClient();
            //var request = new HttpRequestMessage
            //{
            //    RequestUri = new Uri("http://localhost:5000"),
            //    Method = HttpMethod.Get
            //};
            //request.Headers.Add("foo", "123");
            //request.Headers.Add("bar", "456");
            //var response = await httpClient.SendAsync(request);
            //var headers = (await response.Content.ReadAsStringAsync()).Split(";");

            //Debug.Assert(headers.Contains("foo=123"));
            //Debug.Assert(headers.Contains("bar=456"));
            //Debug.Assert(headers.Contains("baz=789"));

            //Console.WriteLine("Hello World!");

            Dictionary<int, string> classList = new Dictionary<int, string>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ITask))))
                       .ToArray();

            Console.WriteLine("ITask实现类列表");

            var i = 1;
            foreach (var v in types)
            {
                Console.WriteLine($"{i}:{v.FullName}");
                classList.Add(i++, v.FullName);
            }
            Console.Write("请选择：");
            var chooseIndex = int.Parse(Console.ReadLine());
            Type obj = Type.GetType(classList[chooseIndex]);
            MethodInfo methodinfo = obj.GetMethod("Print");
            methodinfo.Invoke(Activator.CreateInstance(obj), null);
            Console.Read();
        }
    }
}
