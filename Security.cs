using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce
{
    public class Security
    {
        private static string GetSitePassword()
        {
            return Startup.StaticConfiguration["Encryption:Password"];
        }

        private static string GetSiteIV()
        {
            return Startup.StaticConfiguration["Encryption:IV"];
        }

        public static string Encrypt(string Input)
        {
            try
            {
                using (AesManaged AES = new AesManaged())
                {
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Mode = CipherMode.CBC;

                    PasswordDeriveBytes password = new PasswordDeriveBytes(GetSitePassword(), null);
                    byte[] KeyBytes = password.GetBytes(256 / 8);
                    ICryptoTransform encryptor = AES.CreateEncryptor(KeyBytes, Encoding.UTF8.GetBytes(GetSiteIV()));
                    using (MemoryStream Memory = new MemoryStream())
                    {
                        using (CryptoStream Cryptostream = new CryptoStream(Memory, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter Streamwriter = new StreamWriter(Cryptostream))
                                Streamwriter.Write(Input);
                            return System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(Memory.ToArray()));
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > Security > Encrypt " + Ex.Message);
            }

        }

        public static string Decrypt(string Input)
        {
            try
            {
                Input = System.Web.HttpUtility.UrlDecode(Input);
                var Data = Convert.FromBase64String(Input);

                using (AesManaged AES = new AesManaged())
                {
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Mode = CipherMode.CBC;

                    PasswordDeriveBytes password = new PasswordDeriveBytes(GetSitePassword(), null);
                    byte[] KeyBytes = password.GetBytes(256 / 8);
                    ICryptoTransform decryptor = AES.CreateDecryptor(KeyBytes, Encoding.UTF8.GetBytes(GetSiteIV()));

                    using (MemoryStream Memory = new MemoryStream(Data))
                    {
                        using (CryptoStream Cryptostream = new CryptoStream(Memory, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader Streamreader = new StreamReader(Cryptostream))
                                return Streamreader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > Security > Dencrypt " + Ex.Message);
            }

        }

        public static string SHA512(string input)
        {
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                using (var hash = System.Security.Cryptography.SHA512.Create())
                {
                    var hashedInputBytes = hash.ComputeHash(bytes);
                    var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                    foreach (var b in hashedInputBytes)
                        hashedInputStringBuilder.Append(b.ToString("X2"));
                    return hashedInputStringBuilder.ToString();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > Security > SHA512 " + Ex.Message);
            }

        }

        public static string MD5H(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
