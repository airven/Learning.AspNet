using Common.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.TestConsole
{
    internal class Encrypt: ITask
    {
        private const string ENCRYPT_KEY = "INbSvyvOXkSkcWNSc8HpHIa4";
        //private static final String SECRETKEY = "INbSvyvOXkSkcWNSc8HpHIa4";
        private const string IV = "drS66lwy";
        public void Print()
        {
            string strX = EncryUtils.TripleDesEncryptorCBC("{\"userId\":\"admin\",\"timestamp\":\"1615453746322\"}", ENCRYPT_KEY, IV);
            string strY = EncryUtils.TripleDesDecryptorCBC(strX, ENCRYPT_KEY, IV);

            string param = "bAC70X6ZCjz7vqmrvQeblVgPGediO3n3oynR+O2wqwgoc/qjz54HAEWJvOtE VZvep9lePkYkXNvkAakKl80ZDLM+tXNw2/kfPRvdMQAYm9qGa0yuPaK3bHze DS6ih8oOW4Crh3eIJkKkzY9ukjKyzw==";
            string str3 = EncryUtils.TripleDesDecryptorCBC(param, ENCRYPT_KEY, IV);
            Console.WriteLine(str3);


            str3 = EncryUtils.TripleDesEncryptorCBC(str3, ENCRYPT_KEY, IV);
            Console.WriteLine(str3);
        }
    }
}
