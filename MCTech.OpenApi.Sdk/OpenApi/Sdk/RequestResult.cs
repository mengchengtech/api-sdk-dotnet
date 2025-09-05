using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MCTech.OpenApi.Sdk
{
  public class RequestResult : IDisposable
  {
    private HttpResponseMessage? _response;

    /// <summary>
    /// 返回结果状态码
    /// </summary>
    public HttpStatusCode Status
    {
      get { return this._response!.StatusCode; }
    }

    public string ContentType
    {
      get { return this._response!.Content?.Headers.ContentType?.ToString() ?? ""; }
    }

    /// <summary>
    /// 以字符串方式获取返回的文本内容
    /// </summary>
    /// <returns></returns>
    public string GetString()
    {
      var result = this.GetStringAsync();
      return result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 以字符串方式获取返回的文本内容
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetStringAsync()
    {
      string text = await this.Content().ReadAsStringAsync();
      return text;
    }

    /// <summary>
    /// 以Xml方式获取返回的文本内容
    /// </summary>
    /// <returns></returns>
    public XmlDocument GetXmlDocument()
    {
      var result = this.GetXmlDocumentAsync();
      return result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 以Xml方式获取返回的文本内容
    /// </summary>
    /// <returns></returns>
    public async Task<XmlDocument> GetXmlDocumentAsync()
    {
      XmlDocument doc = new XmlDocument();
      using var r = await this.OpenReadAsync();
      doc.Load(r);
      return doc;
    }

    /// <summary>
    /// 获取Xml方式表示的实体对象。使用XmlSerializer反序列化
    /// </summary>
    /// <typeparam name="T">需要返序列化的对象的C#类</typeparam>
    /// <returns></returns>
    public T? GetXmlObject<T>()
    {
      var result = this.GetXmlObjectAsync(typeof(T));
      return (T?)result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取Xml方式表示的实体对象。使用XmlSerializer反序列化
    /// </summary>
    /// <param name="targetType">需要返序列化的对象的C#类</param>
    /// <returns></returns>
    public object? GetXmlObject(Type targetType)
    {
      var result = this.GetXmlObjectAsync(targetType);
      return result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取Xml方式表示的实体对象。使用XmlSerializer反序列化
    /// </summary>
    /// <typeparam name="T">需要返序列化的对象的C</typeparam>
    /// <returns></returns>
    public async Task<T?> GetXmlObjectAsync<T>()
    {
      var result = await this.GetXmlObjectAsync(typeof(T));
      return (T?)result;
    }

    /// <summary>
    /// 获取Xml方式表示的实体对象。使用XmlSerializer反序列化
    /// </summary>
    /// <param name="targetType">需要返序列化的对象的C#类</param>
    /// <returns></returns>
    public async Task<object?> GetXmlObjectAsync(Type targetType)
    {
      XmlSerializer ser = new XmlSerializer(targetType);
      using var r = await this.OpenReadAsync();
      return ser.Deserialize(r);
    }

    /// <summary>
    /// 获取Json方式表示的实体对象。使用Newtonsoft反序列化
    /// </summary>
    /// <typeparam name="T">需要返序列化的对象的C#类</typeparam>
    /// <returns></returns>
    public T? GetJsonObject<T>()
    {
      var result = this.GetJsonObjectAsync(typeof(T));
      return (T?)result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取Json方式表示的实体对象。使用Newtonsoft反序列化
    /// </summary>
    /// <typeparam name="T">需要返序列化的对象的C#类</typeparam>
    /// <returns></returns>
    public async Task<T?> GetJsonObjectAsync<T>()
    {
      var result = await this.GetJsonObjectAsync(typeof(T));
      return (T?)result;
    }

    /// <summary>
    /// 获取Json方式表示的实体对象。使用Newtonsoft反序列化
    /// </summary>
    /// <param name="targetType">需要返序列化的对象的C#类</param>
    /// <returns></returns>
    public object? GetJsonObject(Type targetType)
    {
      var result = this.GetJsonObjectAsync(targetType);
      return result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取Json方式表示的实体对象。使用Newtonsoft反序列化
    /// </summary>
    /// <param name="targetType">需要返序列化的对象的C#类</param>
    /// <returns></returns>
    public async Task<object?> GetJsonObjectAsync(Type targetType)
    {
      JsonSerializer serializer = new JsonSerializer();
      using var r = await this.OpenReadAsync();
      JsonReader reader = new JsonTextReader(r);
      return serializer.Deserialize(reader, targetType);
    }

    /// <summary>
    /// 获取返回结果的StreamReader
    /// </summary>
    /// <returns></returns>
    public StreamReader OpenRead()
    {
      var result = this.OpenReadAsync();
      return result.GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取返回结果的StreamReader
    /// </summary>
    /// <returns></returns>
    public async Task<StreamReader> OpenReadAsync()
    {
      Stream s = await this.Content().ReadAsStreamAsync();
      Encoding encoding = this.GetContentEncoding();
      return new StreamReader(s, encoding);
    }

    private Encoding GetContentEncoding()
    {// 获取 Content-Type 中的 charset
      var charset = this.Content().Headers.ContentType?.CharSet;
      // 转换成 Encoding 对象
      Encoding encoding = Encoding.UTF8;
      if (!string.IsNullOrEmpty(charset))
      {
        try
        {
          encoding = Encoding.GetEncoding(charset);
        }
        catch (ArgumentException)
        {
          // 如果服务端返回了未知的 charset，回退到 UTF-8
          encoding = Encoding.UTF8;
        }
      }
      return encoding;
    }

    /// <summary>
    /// 获取返回结果的内容对象
    /// </summary>
    /// <returns></returns>
    private HttpContent Content() { return this._response!.Content; }

    internal RequestResult(HttpResponseMessage response)
    {
      this._response = response;
    }

    void IDisposable.Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        this._response?.Dispose();
        this._response = null;
      }
    }

    ~RequestResult()
    {
      Dispose(false);
    }
  }
}
