using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace MCTech.OpenApi.Sdk
{
  internal class SignUtility
  {
    private const string OpenApiPrefix = "x-iwop-";

    public static string BuildCanonicalString(SignatureOption option)
    {
      List<string> itemsToSign = new List<string>();
      itemsToSign.Add(option.Method);
      itemsToSign.Add(option.ContentType);
      //itemsToSign.Add(option.ContentMd5);
      itemsToSign.Add(option.Date.ToUniversalTime().ToString("r"));

      var headers = option.Headers;
      List<string> keys = headers.Keys.Where(key => key.StartsWith(OpenApiPrefix, StringComparison.InvariantCultureIgnoreCase))
        .OrderBy(key => key, StringComparer.InvariantCulture)
        .ToList();

      foreach (string key in keys)
      {
        itemsToSign.Add(key + ":" + headers[key]);
      }

      // Add canonical resource
      string canonicalizedResource = BuildCanonicalizedResource(option.ResourceUri);
      itemsToSign.Add(canonicalizedResource);

      return string.Join("\n", itemsToSign);
    }

    private static string BuildCanonicalizedResource(Uri requestUri)
    {
      string canonicalizedResource = requestUri.GetLeftPart(UriPartial.Path);
      if (string.IsNullOrEmpty(requestUri.Query))
      {
        return canonicalizedResource;
      }

      var query = QueryHelpers.ParseQuery(requestUri.Query);
      List<string> parameterNames = query.Keys
        .OrderBy(key => key, StringComparer.InvariantCulture)
        .ToList();

      char separator = '?';
      StringBuilder builder = new StringBuilder(canonicalizedResource);
      foreach (string paramName in parameterNames)
      {
        builder.Append(separator);
        builder.Append(paramName);
        string? paramValue = query[paramName];
        if (!string.IsNullOrEmpty(paramValue))
        {
          builder.Append('=').Append(WebUtility.UrlEncode(paramValue));
        }

        separator = '&';
      }
      return builder.ToString();
    }
  }
}
