using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppDaltonCatalogo.Infrastructure.SQL.Helpers
{
    public class EncryptHelpers
    {
        public static string GetSHA256(string pw)
        {
            SHA256 sHA256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sHA256.ComputeHash(encoding.GetBytes(pw));
            for (int i = 0; i < stream.Length; i++)
                sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public static string GetNewPasswordTemp(int lengthPwsAlg = 35)
        {
            Random rdn = new Random();
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@/)=!.-+:(";
            StringBuilder pwsTmp = new StringBuilder();
            for (int i = 0; i < lengthPwsAlg; i++)
            {
                pwsTmp.Append(caracteres[rdn.Next(caracteres.Length)]);
            }
            return pwsTmp.ToString();
        }
    }
}
