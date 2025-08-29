using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MCTech.OpenApi.Sdk.NUnitTests
{
  [TestFixture]
  public class ApiClientTest
  {
    private Config _config;
    private OpenApiClient _client;

    [SetUp]  // 每个测试方法运行前执行
    public void Setup()
    {
      this._config = new Config();
      _client = new OpenApiClient(this._config.BaseUrl, this._config.AccessId, this._config.SecretKey);
    }

    [Test]
    public void TestGetByHeaderAndReturnResult()
    {
      RequestOption option = RequestOption.NewBuilder()
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddHeader(new Dictionary<string, object> {
                    { "X-iwop-before", "wq666" },
                    { "x-iwop-integration-id", this._config.IntegrationId},
                    { "x-IWOP-after", "wq666"}
              })
              .Build();
      RequestResult result = this._client.Get(this._config.ApiPath, option);
      Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(result.ContentType.Split(";")[0], Is.EqualTo("application/json"));
      JObject? data = result.GetJsonObject<JObject>();
      Assert.That(data, Contains.Key("updateAt"));
      Assert.That(data, Contains.Key("data"));
    }

    [Test]
    public void TestGetByQueryAndApiNotExists()
    {
      RequestOption option = RequestOption.NewBuilder()
              .SignedBy(new SignedByQuery(new QuerySignatureParams(3600)))
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddQuery(new Dictionary<string, object>() {
                    { "X-iwop-before", "wq666" },
                    { "x-iwop-integration-id", this._config.IntegrationId},
                    { "x-IWOP-after", "wq666"}
              })
              .Build();
      RequestResult result = this._client.Get(this._config.ApiPath, option);
      Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(result.ContentType.Split(";")[0], Is.EqualTo("application/json"));
      JObject? data = result.GetJsonObject<JObject>();
      Assert.That(data, Contains.Key("updateAt"));
      Assert.That(data, Contains.Key("data"));
    }

    [Test]
    public void TestPostByHeaderAndApiNotExists()
    {
      RequestOption option = RequestOption.NewBuilder()
              .AddQuery("integratedProjectId", this._config.IntegrationId)
              .AddHeader("x-iwop-integration-id", this._config.IntegrationId)
              .AddHeader("x-forwarded-for", "192.168.1.1")
              .ContentType("application/xml")
              .Body("<body></body>")
              .Build();
      try
      {
        using RequestResult result = this._client.Post(this._config.ApiPath, option);
        Assert.Fail();
      }
      catch (OpenApiResponseException e)
      {
        Assert.That(e.Status, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(e.Error.ClientIP, Is.EqualTo("192.168.1.1"));
        Assert.That(e.Error.Code, Is.EqualTo("SERVICE_NOT_FOUND"));
        Assert.That(e.Error.Message, Is.EqualTo("'POST /api-ex/-itg-/cb/project-wbs/items' 对应的服务不存在。请检查rest请求中的method, path是否与相应api文档中的完全一致"));
      }
    }
  }
}