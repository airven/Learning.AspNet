using Microsoft.VisualStudio.TestTools.UnitTesting;
using Learning.Common.LambdaExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Common.LambraExpression.Tests
{
    [TestClass]
    public class ObjectMapperTest
    {
        [TestMethod]
        public void TransTest()
        {
            People people = new People()
            {
                Id = 11,
                Name = "Wang",
                Age = 31
            };
            var result = ObjectMapper.Trans<People, PeopleCopy>(people);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, people.Name);
        }
    }


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
}