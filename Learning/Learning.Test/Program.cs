using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Learning.Test
{
    class Program
    {
        static void Main(string[] args)
        {
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
