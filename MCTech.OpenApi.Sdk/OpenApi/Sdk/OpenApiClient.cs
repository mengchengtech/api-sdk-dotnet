using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MCTech.OpenApi.Sdk
{
  public class OpenApiClient
  {
    private readonly string _accessId;
    private readonly string _secretKey;
    private readonly Uri _baseUri;
    private readonly HttpClient _client;

    static OpenApiClient()
    {
      ServicePointManager.DefaultConnectionLimit = 512;
    }

    public OpenApiClient(string baseUri, string accessId, string secretKey)
    {
      this._baseUri = new Uri(baseUri);
      if (string.IsNullOrEmpty(accessId))
      {
        throw new OpenApiClientException("accessId不能为null或empty");
      }
      if (string.IsNullOrEmpty(secretKey))
      {
        throw new OpenApiClientException("secret不能为null或empty");
      }

      this._accessId = accessId;
      this._secretKey = secretKey;
      this._client = new HttpClient(new SocketsHttpHandler
      {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),   // 连接存活时间
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2), // 空闲连接超时
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
      });
      this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
      this._client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));
    }

    public RequestResult Get(string pathAndQuery)
    {
      var result = Request(pathAndQuery, null, HttpMethod.Get);
      return result.Result;
    }

    public RequestResult Delete(string pathAndQuery)
    {
      var result = Request(pathAndQuery, null, HttpMethod.Delete);
      return result.Result;
    }

    public RequestResult Post(string pathAndQuery, string body)
    {
      byte[] data = Encoding.UTF8.GetBytes(body);
      var result = Request(pathAndQuery, new MemoryStream(data), HttpMethod.Post);
      return result.Result;
    }

    public RequestResult Post(string pathAndQuery, Stream streamBody)
    {
      var result = Request(pathAndQuery, streamBody, HttpMethod.Post);
      return result.Result;
    }

    public RequestResult Put(string pathAndQuery, string body)
    {
      byte[] data = Encoding.UTF8.GetBytes(body);
      var result = Request(pathAndQuery, new MemoryStream(data), HttpMethod.Put);
      return result.Result;
    }

    public RequestResult Put(string pathAndQuery, Stream streamBody)
    {
      var result = Request(pathAndQuery, streamBody, HttpMethod.Put);
      return result.Result;
    }

    public RequestResult Patch(string pathAndQuery, string body)
    {
      byte[] data = Encoding.UTF8.GetBytes(body);
      var result = Request(pathAndQuery, new MemoryStream(data), HttpMethod.Patch);
      return result.Result;
    }

    public RequestResult Patch(string pathAndQuery, Stream streamBody)
    {
      var result = Request(pathAndQuery, streamBody, HttpMethod.Patch);
      return result.Result;
    }

    private async Task<RequestResult> Request(string pathAndQuery, Stream? streamBody, HttpMethod method)
    {
      HttpRequestMessage req = CreateRequestMessage(pathAndQuery, method);
      if (streamBody != null)
      {
        req.Content = new StreamContent(streamBody);
      }
      HttpResponseMessage response = await this._client.SendAsync(req);
      return new RequestResult(response);
    }

    private HttpRequestMessage CreateRequestMessage(string pathAndQuery, HttpMethod method)
    {
      Uri apiUri = new Uri(this._baseUri, pathAndQuery);

      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUri);
      request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      var now = DateTimeOffset.Now;
      request.Headers.Date = now;
      var contentType = "application/json";
      request.Headers.Add(HttpRequestHeader.ContentType.ToString(), contentType);
      request.Method = method;
      SignatureOption option = new SignatureOption(request.RequestUri!, method, contentType, now);
      string canonicalString = Utility.BuildCanonicalString(option);
      HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_secretKey));
      byte[] byteText = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalString));
      string signature = Convert.ToBase64String(byteText);
      request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "IWOP " + this._accessId + ":" + signature);

      return request;
    }
  }
}
