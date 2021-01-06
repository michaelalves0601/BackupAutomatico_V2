using BackupNuvemSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackupNuvemSBuild_Configuration
{
    class Encrypt
    {
        Log log = new Log();
        
        public string Publickey { get; }
        public string Privatekey { get; }

        public Encrypt(string publickey, string privatekey)
        {
            this.Publickey = publickey;
            this.Privatekey = privatekey;
        }


        public string Encrypto(string textToEncrypt)
        {
            string ToReturn = "";
            try
            {
                
                byte[] secretkeyByte = { };
                secretkeyByte = Encoding.UTF8.GetBytes(Privatekey);
                byte[] publickeybyte = { };
                publickeybyte = Encoding.UTF8.GetBytes(Publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                
            }
            catch (Exception ex)
            {
                log.LogError("Erro de criptografia");
            }
            return ToReturn;
        }


        public string Decrypto(string textToDecrypt)
        { 
            string ToReturn = "";
            try
            {
                
                byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(Privatekey);
                byte[] publickeybyte = { };
                publickeybyte = Encoding.UTF8.GetBytes(Publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF7;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                
            }
            catch (Exception ae)
            {
                log.LogError("Erro de criptografia");
            }
            return ToReturn;
        }

    }
}
