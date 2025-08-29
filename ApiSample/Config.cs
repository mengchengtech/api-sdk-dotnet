using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ApiSample
{
  internal class Config
  {
    public string AccessId { get; set; }

    public string SecretKey { get; set; }

    public string BaseUrl { get; set; }

    public string ApiPath { get; set; }

    public string IntegrationId { get; set; }

    public Config()
    {
      this.AccessId = "{accessId}";
      this.SecretKey = "{secretKey}";
      this.BaseUrl = "{baseUrl}";
      this.ApiPath = "{apiPath}";
      this.IntegrationId = "{integrationId}";
    }
  }
}