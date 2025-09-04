using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections;
using System.Xml;
using Microsoft.Net.Http.Headers;

namespace MCTech.OpenApi.Sdk
{
  internal static class Utility
  {
    private static IDictionary<string, string> GetCustomMap<T>(IEnumerable<KeyValuePair<string, T>> pairs) where T : IEnumerable
    {
      IDictionary<string, string> iwopValues = new Dictionary<string, string>();
      foreach (var (key, value) in pairs)
      {
        string lowerCaseName = key.ToLower();
        if (lowerCaseName.StartsWith(Constants.CUSTOM_PREFIX))
        {
          iwopValues[lowerCaseName] = string.Join(",", value.Cast<string?>());
        }
      }
      return iwopValues;
    }

    private static string GetResource(Uri requestUri)
    {
      if (string.IsNullOrEmpty(requestUri.Query))
      {
        return requestUri.ToString();
      }

      var @params = QueryHelpers.ParseQuery(requestUri.Query);
      foreach (string key in @params.Keys)
      {
        // 排除掉表用于认证的固定参数
        if (Constants.QUERY_KEYS.Contains(key))
        {
          @params.Remove(key);
          continue;
        }

        // 排除掉特定前缀的参数，例如 'x-iwop-'
        string lowerCaseName = key.ToLower();
        if (lowerCaseName.StartsWith(Constants.CUSTOM_PREFIX))
        {
          @params.Remove(key);
        }
      }
      UriBuilder uriBuilder = new UriBuilder(requestUri);
      uriBuilder.Query = string.Join("&",
        @params.OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
          .SelectMany(kvp => kvp.Value.Select(v => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(v!)}"))
        );
      return uriBuilder.Uri.ToString();
    }

    private static SignedData ComputeSignature(SignatureMode mode, SignatureOption option, string time)
    {
      List<string> signableItems = new List<string>();
      signableItems.Add(option.Method.ToString());
      if (!string.IsNullOrEmpty(option.ContentType))
      {
        signableItems.Add(option.ContentType!);
      }
      signableItems.Add(time);
      IDictionary<string, string>? customMap = null;
      if (mode == SignatureMode.Header)
      {
        customMap = GetCustomMap(option.Headers);
      }
      else if (mode == SignatureMode.Query)
      {
        customMap = GetCustomMap(QueryHelpers.ParseQuery(option.RequestUri.Query));
      }
      if (customMap != null)
      {
        List<string> keys = customMap.Keys
            .Order()
            .ToList();
        foreach (string key in keys)
        {
          signableItems.Add(key + ":" + customMap[key]);
        }
      }

      string canonicalizedResource = GetResource(option.RequestUri);
      signableItems.Add(canonicalizedResource);

      string signable = string.Join("\n", signableItems);
      string signature = HmacSha1(signable, option.Secret);
      return new SignedData(signable, signature);
    }

    private static string HmacSha1(string signable, string secret)
    {
      HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
      byte[] byteText = hmac.ComputeHash(Encoding.UTF8.GetBytes(signable));
      return Convert.ToBase64String(byteText);
    }

    public static ApiGatewayErrorData ResolveError(Stream s)
    {
      XmlDocument document = new XmlDocument();
      document.Load(s);
      XmlNodeList items = document.DocumentElement!.ChildNodes;
      Hashtable map = new Hashtable();
      foreach (XmlElement item in items)
      {
        string name = item.Name;
        string value = item.InnerText;
        map.Add(name, value);
      }

      return new ApiGatewayErrorData(map);
    }

    public static SignedInfo GenerateSignature(ISignedBy signedBy, SignatureOption option)
    {
      if (string.IsNullOrEmpty(option.AccessId))
      {
        throw new OpenApiClientException("accessId不能为null或empty");
      }
      if (string.IsNullOrEmpty(option.Secret))
      {
        throw new OpenApiClientException("secret不能为null或empty");
      }
      var method = option.Method;
      if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
      {
        if (string.IsNullOrWhiteSpace(option.ContentType))
        {
          throw new OpenApiClientException("http请求缺少'content-type'头。请求方式为[" + method + "]时，需要在RpcInvoker的headers属性上设置'content-type'");
        }
      }
      string time;
      IDictionary<string, string>? query = null;
      IDictionary<string, string>? headers = null;
      if (signedBy.Mode == SignatureMode.Query)
      {
        QuerySignatureParams? p = ((SignedByQuery)signedBy).Parameters;
        long d = p?.Duration > 0 ? (long)p.Duration : Constants.DEFAULT_EXPIRES;
        long expires = d + DateTimeOffset.Now.ToUnixTimeSeconds();
        time = expires.ToString();
        query = new Dictionary<string, string>
        {
          { Constants.QUERY_ACCESS_ID , option.AccessId },
          { Constants.QUERY_EXPIRES, time },
          { Constants.QUERY_SIGNATURE, "" },
        };
      }
      else
      {
        time = HeaderUtilities.FormatDate(DateTimeOffset.Now);
        headers = new Dictionary<string, string> {
          { HttpRequestHeader.Date.ToString(), time },
          { HttpRequestHeader.Authorization.ToString(), "" }
        };
      }
      SignedData signed = ComputeSignature(signedBy.Mode, option, time);
      if (query != null)
      {
        query[Constants.QUERY_SIGNATURE] = signed.Signature;
      }
      else
      {
        headers![HttpRequestHeader.Authorization.ToString()] = "IWOP " + option.AccessId + ":" + signed.Signature;
      }
      return new SignedInfo(signedBy.Mode, signed, headers, query);
    }
  }
}
