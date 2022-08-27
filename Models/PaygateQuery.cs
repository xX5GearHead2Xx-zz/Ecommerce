using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Ecommerce.Models
{
    public class PaygateQuery
    {
        #region "Properties"
        private string PaygateID { get; set; }
        public string PayRequestID { get; set; }
        public string Reference { get; set; }
        public string Checksum { get; set; }
        #endregion

        public PaygateQuery()
        {
            PaygateID = Startup.StaticConfiguration["PaygateSettings:Key"];
            PayRequestID = "";
            Reference = "";
            Checksum = "";
        }

        public Dictionary<string, string> QueryTransaction()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Startup.StaticConfiguration["PaygateSettings:QueryURL"]);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";
            StringBuilder PostData = new StringBuilder();
            PostData.Append("PAYGATE_ID=" + PaygateID + "");
            PostData.Append("&PAY_REQUEST_ID=" + PayRequestID + "");
            PostData.Append("&REFERENCE=" + Reference + "");
            PostData.Append("&CHECKSUM=" + Checksum + "");
            var data = Encoding.ASCII.GetBytes(PostData.ToString());

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseContent.Split('&').Select(d => d.Split('=')).ToDictionary(sp => sp[0], sp => sp[1]);
        }
    }


}
