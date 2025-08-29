using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace MCTech.OpenApi.Sdk
{
  internal class SignatureOption
  {
    public string AccessId { get; private set; }
    public string Secret { get; private set; }
    /// <summary>
    /// 获取或设置REST调用签名中的url路径信息
    /// </summary>
    public Uri RequestUri { get; private set; }
    /// <summary>
    /// 获取或设置设置REST调用签名中的method信息
    /// </summary>
    public HttpMethod Method { get; private set; }
    /// <summary>
    /// 获取或设置REST调用中的content-type头
    /// </summary>
    public string? ContentType { get; private set; }
    /// <summary>
    /// headers头
    /// </summary>
    public HttpRequestHeaders Headers { get; private set; }

    public SignatureOption(string accessId, string secret, Uri requestUri, HttpMethod method, string? contentType, HttpRequestHeaders headers)
    {
      this.AccessId = accessId;
      this.Secret = secret;
      this.RequestUri = requestUri;
      this.Method = method;
      this.ContentType = contentType;
      this.Headers = headers;
    }
  }
}
