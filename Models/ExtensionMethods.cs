using System;
namespace Ecommerce.Models
{
    public static class ExtensionMethods
    {
        public static string SanitizeInput(this string value)
        {
            return value.Replace("'", "''");
        }

        public static string Encrypt(this string value)
        {
            return Security.Encrypt(value);
        }

        public static string Decrypt(this string value)
        {
            return Security.Decrypt(value);
        }

        public static string ToDBDate(this DateTime Date)
        {
            return Date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToDBDate(this DateTime? Date)
        {
            if(Date.HasValue)
            {
                return Convert.ToDateTime(Date).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return "";
            }
        }
    }
}
