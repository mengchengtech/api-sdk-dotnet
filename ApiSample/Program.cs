using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

using log4net;
using MCTech.OpenApi.Sdk;
using Newtonsoft.Json;

namespace ApiSample
{
  class Program
  {
    private static readonly ILog logger = LogManager.GetLogger("logger");
    private readonly Config _config;
    private readonly OpenApiClient _client;

    private Program()
    {
      this._config = new Config();
      this._client = new OpenApiClient(this._config.BaseUrl, this._config.AccessId, this._config.SecretKey);
    }

    static void Main(string[] args)
    {
      Program app = new Program();
      app.TestGetByHeader();
      app.TestGetByQuery();
      app.TestPostByHeader();
    }
    private void TestGetByHeader()
    {
      RequestOption option = RequestOption.NewBuilder()
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddHeader(new Dictionary<string, object> {
                    { "X-iwop-before", "wq666" },
                    { "x-iwop-integration-id", this._config.IntegrationId},
                    { "x-IWOP-after", "wq666"}
              })
              .Build();
      try
      {
        using RequestResult result = this._client.Get(this._config.ApiPath, option);
        Console.WriteLine(result.GetString());
      }
      catch (OpenApiClientException e)
      {
        logger.Error(e.Message, e);
      }
      catch (OpenApiResponseException e)
      {
        logger.Error(e.Message, e);
        ApiGatewayErrorData error = e.Error;
        // TODO: 处理api网关返回的异常
        logger.Error(JsonConvert.SerializeObject(error));
      }
    }

    private void TestGetByQuery()
    {
      RequestOption option = RequestOption.NewBuilder()
              .SignedBy(new SignedByQuery(new QuerySignatureParams(3600)))
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddQuery(new Dictionary<string, object> {
                    { "X-iwop-before", "wq666" },
                    { "x-iwop-integration-id", this._config.IntegrationId},
                    { "x-IWOP-after", "wq666"}
              })
                .Build();
      try
      {
        using RequestResult result = this._client.Get(this._config.ApiPath, option);
        Console.WriteLine(result.GetString());
      }
      catch (OpenApiClientException e)
      {
        logger.Error(e.Message, e);
      }
      catch (OpenApiResponseException e)
      {
        logger.Error(e.Message, e);
        ApiGatewayErrorData error = e.Error;
        // TODO: 处理api网关返回的异常
        logger.Error(JsonConvert.SerializeObject(error));
      }
    }

    private void TestPostByHeader()
    {
      RequestOption option = RequestOption.NewBuilder()
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddHeader("x-iwop-integration-id", this._config.IntegrationId)
              .ContentType("application/xml")
              .Body("<body></body>")
              .Build();
      try
      {
        using RequestResult result = this._client.Post(this._config.ApiPath, option);
        Console.WriteLine(result.GetString());
      }
      catch (OpenApiClientException e)
      {
        logger.Error(e.Message, e);
      }
      catch (OpenApiResponseException e)
      {
        logger.Error(e.Message, e);
        ApiGatewayErrorData error = e.Error;
        // TODO: 处理api网关返回的异常
        logger.Error(JsonConvert.SerializeObject(error));
      }
    }
  }
}
