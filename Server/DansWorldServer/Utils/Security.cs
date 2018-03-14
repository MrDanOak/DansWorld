using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DansWorld.Server.Utils
{
    class Security
    {
        //https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create(); 
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
