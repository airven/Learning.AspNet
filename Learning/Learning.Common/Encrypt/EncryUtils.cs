using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;

namespace Common.Encrypt
{
    /// <summary>
    /// 加解密帮助类(3DES加密方法:CBC模式、ECB模式、)
    /// </summary>
    public class EncryUtils
    {
        /// <summary>
        /// 3DES加密CBC模式
        /// </summary>
        /// <param name="text">明文（待加密）</param>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string TripleDesEncryptorCBC(string text, string key, string iv)
        {
            var tripleDESCipher = new TripleDESCryptoServiceProvider();
            tripleDESCipher.Mode = CipherMode.CBC;
            tripleDESCipher.Padding = PaddingMode.PKCS7;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[24];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            tripleDESCipher.Key = keyBytes;
            tripleDESCipher.IV = Encoding.ASCII.GetBytes(iv);

            ICryptoTransform transform = tripleDESCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// 3DES解密CBC模式
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string TripleDesDecryptorCBC(string text, string key, string iv)
        {
            var tripleDESCipher = new TripleDESCryptoServiceProvider();
            tripleDESCipher.Mode = CipherMode.CBC;
            tripleDESCipher.Padding = PaddingMode.PKCS7;

            byte[] encryptedData = Convert.FromBase64String(text);
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[24];
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv);
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            tripleDESCipher.Key = keyBytes;
            tripleDESCipher.IV = ivBytes;
            ICryptoTransform transform = tripleDESCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// 3DES加密
        /// base64小细节，当使用get请求时，base64生成字符中有“+”号，
        /// 注意需要转换“%2B”，否则会被替换成空格。POST不存在
        /// while (str.IndexOf('+') != -1) {
        ///	 str = str.Replace("+","%2B");
        ///  }
        /// </summary>
        public static string Des3EncodeECB(string text, string key)
        {

            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    Encoding encoding = Encoding.GetEncoding("UTF-8");
                    var DES = new TripleDESCryptoServiceProvider();
                    DES.Key = encoding.GetBytes(key);
                    DES.Mode = CipherMode.ECB;
                    ICryptoTransform DESEncrypt = DES.CreateEncryptor();
                    byte[] Buffer = encoding.GetBytes(text);
                    return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
                }
            }
            catch (Exception ex)
            {
                return string.Empty;

            }
            return string.Empty;
        }

        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <returns>解密串</returns>
        /// <param name="a_strString">加密串</param>
        public static string Des3DecodeECB(string text, string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var DES = new TripleDESCryptoServiceProvider();
                    DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
                    DES.Mode = CipherMode.ECB;
                    DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                    byte[] Buffer = Convert.FromBase64String(text);
                    return Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// DESEncrypt
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DESEncrypt(string paymentCode, string key, string iv)
        {
            SymmetricAlgorithm symmetric;
            ICryptoTransform iCrypto;
            MemoryStream memory;
            CryptoStream crypto;
            byte[] byt;
            symmetric = new TripleDESCryptoServiceProvider();
            symmetric.Key = Encoding.UTF8.GetBytes(key);
            symmetric.IV = Encoding.UTF8.GetBytes(iv);
            iCrypto = symmetric.CreateEncryptor();
            byt = Encoding.UTF8.GetBytes(paymentCode);
            memory = new MemoryStream();
            crypto = new CryptoStream(memory, iCrypto, CryptoStreamMode.Write);
            crypto.Write(byt, 0, byt.Length);
            crypto.FlushFinalBlock();
            crypto.Close();
            return Convert.ToBase64String(memory.ToArray());
        }

        /// <summary>
        /// DESDecrypst
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DESDecrypst(string data, string key, string iv)
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

            mCSP.Key = Encoding.UTF8.GetBytes(key);
            mCSP.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform iCrypto;
            MemoryStream memory;
            CryptoStream crypto;
            byte[] byt;
            iCrypto = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(data);
            memory = new MemoryStream();
            crypto = new CryptoStream(memory, iCrypto, CryptoStreamMode.Write);
            crypto.Write(byt, 0, byt.Length);
            crypto.FlushFinalBlock();
            crypto.Close();
            return Encoding.UTF8.GetString(memory.ToArray());
        }

    }
}
