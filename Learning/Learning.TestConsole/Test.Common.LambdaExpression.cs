using Common.LambdaExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.TestConsole
{
    public class LambdaExpression:ITask
    {
        class People
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public int Id { get; set; }
        }

        class PeopleCopy
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public int Id { get; set; }
        }
        public void Print()
        {
            People people = new People()
            {
                Id = 11,
                Name = "Wang",
                Age = 31
            };
            var result = ObjectMapper.Trans<People, PeopleCopy>(people);
            Console.Read();
        }
    }
}
