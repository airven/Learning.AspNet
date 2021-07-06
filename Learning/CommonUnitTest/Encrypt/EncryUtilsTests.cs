using Microsoft.VisualStudio.TestTools.UnitTesting;
using Learning.Common.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Common.Encrypt.Tests
{
    [TestClass, TestCategory("EncryUtilsTests")]
    public class EncryUtilsTests
    {
        private const string ENCRYPT_KEY = "INbSvyvOXkSkcWNSc8HpHIa4";
        private const string IV = "drS66lwy";

        [DataTestMethod]
        [DataRow("{\"userId\":\"admin\",\"timestamp\":\"1615453746322\"}", ENCRYPT_KEY, IV)]
        public void TripleDesEncryptorCBCTest(string text, string key, string iv)
        {
            string strX = EncryUtils.TripleDesEncryptorCBC(text, key, iv);
            string strY = EncryUtils.TripleDesDecryptorCBC(strX, ENCRYPT_KEY, IV);
            Assert.AreEqual(strY, text);
        }

        [TestMethod]
        public void TripleDesDecryptorCBCTest()
        {
            string param = "bAC70X6ZCjz7vqmrvQeblVgPGediO3n3oynR+O2wqwgoc/qjz54HAEWJvOtE VZvep9lePkYkXNvkAakKl80ZDLM+tXNw2/kfPRvdMQAYm9qGa0yuPaK3bHze DS6ih8oOW4Crh3eIJkKkzY9ukjKyzw==";
            string str3 = EncryUtils.TripleDesDecryptorCBC(param, ENCRYPT_KEY, IV);
            Assert.IsTrue(!string.IsNullOrEmpty(str3));
        }
    }
}