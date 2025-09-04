using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace MCTech.OpenApi.Sdk
{
  public class OpenApiClient
  {
    private const string CONTENT_TYPE_VALUE = "application/json; charset=UTF-8";
    private const string ACCEPT_VALUE = "application/json, application/xml, */*";

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
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
      });
      this._client.DefaultRequestHeaders.Accept.ParseAdd(ACCEPT_VALUE);
      this._client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-CN");
    }

    public RequestResult Get(string apiPath, RequestOption option)
    {
      return Request(HttpMethod.Get, apiPath, option);
    }

    public async Task<RequestResult> GetAsync(string apiPath, RequestOption option)
    {
      return await this.RequestAsync(HttpMethod.Get, apiPath, option);
    }

    public RequestResult Delete(string apiPath, RequestOption option)
    {
      return Request(HttpMethod.Delete, apiPath, option);
    }

    public async Task<RequestResult> DeleteAsync(string apiPath, RequestOption option)
    {
      return await this.RequestAsync(HttpMethod.Delete, apiPath, option);
    }

    public RequestResult Post(string apiPath, RequestOption option)
    {
      return Request(HttpMethod.Post, apiPath, option);
    }

    public async Task<RequestResult> PostAsync(string apiPath, RequestOption option)
    {
      return await this.RequestAsync(HttpMethod.Post, apiPath, option);
    }

    public RequestResult Put(string apiPath, RequestOption option)
    {
      return Request(HttpMethod.Put, apiPath, option);
    }

    public async Task<RequestResult> PutAsync(string apiPath, RequestOption option)
    {
      return await this.RequestAsync(HttpMethod.Put, apiPath, option);
    }

    public RequestResult Patch(string apiPath, RequestOption option)
    {
      return Request(HttpMethod.Patch, apiPath, option);
    }

    public async Task<RequestResult> PatchAsync(string apiPath, RequestOption option)
    {
      return await this.RequestAsync(HttpMethod.Patch, apiPath, option);
    }

    public RequestResult Request(HttpMethod method, string apiPath, RequestOption option)
    {
      var result = this.RequestAsync(method, apiPath, option);
      return result.GetAwaiter().GetResult();
    }

    public async Task<RequestResult> RequestAsync(HttpMethod method, string apiPath, RequestOption option)
    {
      HttpRequestMessage req = new HttpRequestMessage(method, (string?)null);
      HttpContent? entity = option.Entity;
      if (entity != null)
      {
        req.Content = entity;
        string? contentType = option.ContentType;
        if (string.IsNullOrEmpty(contentType))
        {
          contentType = CONTENT_TYPE_VALUE;
        }
        req.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType!);
      }
      Uri apiUri = new Uri(this._baseUri, apiPath);
      if (option.Query.Count > 0)
      {
        UriBuilder urlBuilder = new UriBuilder(apiUri);
        urlBuilder.Query = QueryHelpers.AddQueryString(urlBuilder.Query, option.Query);
        apiUri = urlBuilder.Uri;
      }
      req.RequestUri = apiUri;
      if (option.Headers.Count > 0)
      {
        foreach (var (name, value) in option.Headers)
        {
          req.Headers.Remove(name);
          req.Headers.Add(name, value);
        }
      }

      this.MakeSignature(req, option.SignedBy);

      int? timeout = option.Timeout;
      HttpResponseMessage response;
      if (timeout != null && timeout > 0)
      {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds((int)timeout));
        response = await this._client.SendAsync(req, cts.Token);
      }
      else
      {
        response = await this._client.SendAsync(req);
      }

      if (!response.IsSuccessStatusCode)
      {
        Stream xmlContent = response.Content.ReadAsStream();
        ApiGatewayErrorData error = Utility.ResolveError(xmlContent);
        throw new OpenApiResponseException(error.Message, response.StatusCode, error);
      }
      return new RequestResult(response);
    }

    private void MakeSignature(HttpRequestMessage request, ISignedBy? signedBy)
    {
      var contentType = request.Content?.Headers.ContentType;
      SignatureOption option = new SignatureOption(
        this._accessId,
        this._secretKey,
        request.RequestUri!,
        request.Method,
        contentType?.ToString(),
        request.Headers
      );

      if (signedBy == null)
      {
        signedBy = new SignedByHeader();
      }
      SignedInfo signedInfo = Utility.GenerateSignature(signedBy, option);
      if (signedInfo.Headers != null)
      {
        foreach (var (key, value) in signedInfo.Headers!)
        {
          request.Headers.Remove(key);
          request.Headers.Add(key, value);
        }
      }

      if (signedInfo.Query != null)
      {
        UriBuilder urlBuilder = new UriBuilder(request.RequestUri!);
        urlBuilder.Query = QueryHelpers.AddQueryString(urlBuilder.Query, signedInfo.Query!);
        request.RequestUri = urlBuilder.Uri;
      }
    }
  }
}
