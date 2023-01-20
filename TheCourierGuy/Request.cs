using System.Net.Http;
using System.Threading.Tasks;
using System;
using Aws4RequestSigner;

namespace Ecommerce.TheCourierGuy
{
    internal class Request
    {
        private string RequestUri { get; set; }
        private StringContent? Content { get; set; }
        private HttpMethod Method { get; set; }
        private string AccessKey { get; set; }
        private string SecretAccessKey { get; set; }
        private string Region { get; set; }
        public Request(HttpMethod method, string requestUri, StringContent? content = null)
        {
            Method = method;
            Content = content;
            RequestUri = requestUri;
            AccessKey = "AKIA55D5DNTBHX63Z44V";
            SecretAccessKey = "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";
            Region = "af-south-1";
        }

        public async Task<string> Send()
        {
            var signer = new AWS4RequestSigner(AccessKey, SecretAccessKey);
            HttpRequestMessage Request = new HttpRequestMessage(Method, new Uri(RequestUri));
            if (Content != null)
            {
                Request.Content = Content;
            }
            Request = await signer.Sign(Request, "execute-api", Region);
            var client = new HttpClient();
            var response = await client.SendAsync(Request);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
