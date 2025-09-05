using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MCTech.OpenApi.Sdk.NUnitTests
{
  internal class Config
  {
    public string AccessId
    {
      get
      {
        return this._jo?["credential"]?["accessId"]?.Value<string>() ?? "{accessId}";
      }
    }

    public string SecretKey
    {
      get
      {
        return this._jo?["credential"]?["secretKey"]?.Value<string>() ?? "{secretKey}";
      }
    }

    public string BaseUrl
    {
      get
      {
        return this._jo?["baseUrl"]?.Value<string>() ?? "{baseUrl}";
      }
    }

    public string ApiPath
    {
      get
      {
        return this._jo?["apiPath"]?.Value<string>() ?? "{apiPath}";
      }
    }

    public string IntegrationId
    {
      get
      {
        return this._jo?["integrationId"]?.Value<string>() ?? "{integrationId}";
      }
    }

    private JObject? _jo;

    public Config()
    {
      string rootDir = GetSolutionDir();
      using Stream s = File.Open(Path.Join(rootDir, "config.json"), FileMode.Open);
      JsonSerializer ser = new JsonSerializer();
      this._jo = (JObject?)ser.Deserialize(new JsonTextReader(new StreamReader(s, Encoding.UTF8)));
    }

    private static string GetSolutionDir()
    {
      var dir = new DirectoryInfo(AppContext.BaseDirectory);
      while (dir != null && !File.Exists(Path.Combine(dir.FullName, "*.sln")))
      {
        // 判断当前目录下有没有 .sln 文件
        var hasSln = dir.GetFiles("*.sln").Length > 0;
        if (hasSln)
          return dir.FullName;

        dir = dir.Parent;
      }
      throw new DirectoryNotFoundException("Solution directory not found.");
    }
  }
}