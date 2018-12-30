using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MCTech.OpenApi.Sdk
{
    public class OpenApiClient
    {
        private string _accessId;
        private string _secretKey;
        private Uri _baseUri;

        static OpenApiClient()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
        }

        public OpenApiClient(string baseUri, string accessId, string secretKey)
        {
            this._baseUri = new Uri(baseUri);
            if (string.IsNullOrEmpty(accessId)) {
                throw new MCTechOpenApiException("accessId不能为null或empty");
            }
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new MCTechOpenApiException("secret不能为null或empty");
            }

            this._accessId = accessId;
            this._secretKey = secretKey;
        }

        public RequestResult Get(string pathAndQuery)
        {
            return SendRequest(pathAndQuery, null, HttpMethod.Get);
        }

        public RequestResult Delete(string pathAndQuery)
        {
            return SendRequest(pathAndQuery, null, HttpMethod.Delete);
        }

        public RequestResult Post(string pathAndQuery, string body)
        {
            byte[] data = Encoding.UTF8.GetBytes(body);
            return SendRequest(pathAndQuery, new MemoryStream(data), HttpMethod.Post);
        }

        public RequestResult Post(string pathAndQuery, Stream streamBody)
        {
            return SendRequest(pathAndQuery, streamBody, HttpMethod.Post);
        }

        public RequestResult Put(string pathAndQuery, string body)
        {
            byte[] data = Encoding.UTF8.GetBytes(body);
            return SendRequest(pathAndQuery, new MemoryStream(data), HttpMethod.Put);
        }

        public RequestResult Put(string pathAndQuery, Stream streamBody)
        {
            return SendRequest(pathAndQuery, streamBody, HttpMethod.Put);
        }

        public RequestResult Patch(string pathAndQuery, string body)
        {
            byte[] data = Encoding.UTF8.GetBytes(body);
            return SendRequest(pathAndQuery, new MemoryStream(data), HttpMethod.Patch);
        }

        public RequestResult Patch(string pathAndQuery, Stream streamBody)
        {
            return SendRequest(pathAndQuery, streamBody, HttpMethod.Patch);
        }

        private RequestResult SendRequest(string pathAndQuery, Stream streamBody, string method)
        {
            HttpWebRequest webReq = createRequest(pathAndQuery, method);
            webReq.Method = method;
            if (streamBody != null)
            {
                streamBody.CopyTo(webReq.GetRequestStream());
            }
            try 
            {
                HttpWebResponse response = webReq.GetResponse() as HttpWebResponse;
                return new RequestResult(response);
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                return new RequestResult(response);
            }
        }

        private HttpWebRequest createRequest(string pathAndQuery, string method)
        {
            Uri apiUri = new Uri(this._baseUri, pathAndQuery);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUri);
            request.Date = DateTime.Now;
            request.Accept = "application/json, application/xml";
            request.Headers.Add("Accept-Language", "zh-CN");
            request.ContentType = "application/json; charset=UTF-8";
            request.KeepAlive = true;
            request.Headers.Add(HttpRequestHeader.KeepAlive, "3000");
            request.Method = method;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            string httpMethod = request.Method.ToUpperInvariant();
            SignatureOption option = new SignatureOption(request.RequestUri, request.Method, request.ContentType, request.Date);
            string canonicalString = SignUtility.BuildCanonicalString(option);
            HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_secretKey));
            byte[] byteText = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalString));
            string signature = Convert.ToBase64String(byteText);
            request.Headers[HttpConsts.Authorization] =  "IWOP " + this._accessId + ":" + signature;

            return request;
        }
    }
}
