using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Ecommerce.Models
{
    public class PaygateRequest
    {
        #region "Properties"
        private string PaygateID { get; set; }
        private string Reference { get; set; }
        private int Amount { get; set; }
        private string Currency { get; set; }
        private string ReturnURL { get; set; }
        private DateTime TransactionDate { get; set; }
        private string Locale { get; set; }
        private string Country { get; set; }
        private string Email { get; set; }
        private string Checksum { get; set; }
        #endregion

        public PaygateRequest(int amount, string reference, string email)
        {
            PaygateID = Startup.StaticConfiguration["PaygateSettings:Key"];
            Reference = reference;
            Amount = amount;
            Currency = Startup.StaticConfiguration["PaygateSettings:Currency"];
            ReturnURL = Startup.StaticConfiguration["PaygateSettings:ReturnURL"];
            TransactionDate = DateTime.Now;
            Locale = Startup.StaticConfiguration["PaygateSettings:Locale"];
            Country = Startup.StaticConfiguration["PaygateSettings:Country"];
            Email = email;
            Checksum = CalculateChecksum();
        }

        public Dictionary<string, string> InitiateTransaction()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Startup.StaticConfiguration["PaygateSettings:InitiateURL"]);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";
            StringBuilder PostData = new StringBuilder();
            PostData.Append("PAYGATE_ID=" + PaygateID + "");
            PostData.Append("&REFERENCE=" + Reference + "");
            PostData.Append("&AMOUNT=" + (Amount * 100) + "");
            PostData.Append("&CURRENCY=" + Currency + "");
            PostData.Append("&RETURN_URL=" + ReturnURL + "");
            PostData.Append("&TRANSACTION_DATE=" + TransactionDate.ToString("yy-MM-dd HH:mm:ss") + "");
            PostData.Append("&LOCALE=" + Locale + "");
            PostData.Append("&COUNTRY=" + Country + "");
            PostData.Append("&EMAIL="+Email+"");
            PostData.Append("&CHECKSUM="+Checksum+"");
            var data = Encoding.ASCII.GetBytes(PostData.ToString());

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseContent.Split('&').Select(d => d.Split('=')).ToDictionary(sp => sp[0], sp => sp[1]);
        }

        private string CalculateChecksum()
        {
            StringBuilder Parameters = new StringBuilder();
            Parameters.Append(PaygateID);
            Parameters.Append(Reference);
            Parameters.Append(Amount * 100);
            Parameters.Append(Currency);
            Parameters.Append(ReturnURL);
            Parameters.Append(TransactionDate.ToString("yy-MM-dd HH:mm:ss"));
            Parameters.Append(Locale);
            Parameters.Append(Country);
            Parameters.Append(Email);
            Parameters.Append(Startup.StaticConfiguration["PaygateSettings:Password"]);
            return Security.MD5H(Parameters.ToString());
        }
    }
}
