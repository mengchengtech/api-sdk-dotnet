using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace MCTech.OpenApi.Sdk
{
  public class RequestResult : IDisposable
  {
    private HttpResponseMessage? _response;
    /// <summary>
    /// ��ȡ�����÷��ص���Ӧ���ݵı���
    /// </summary>
    public Encoding Encoding { get; private set; }

    /// <summary>
    /// ���ؽ��״̬��
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// ���ַ�����ʽ��ȡ���ص��ı�����
    /// </summary>
    /// <returns></returns>
    public string GetContent()
    {
      using Stream s = this.HttpContent().ReadAsStream();
      StreamReader reader = new StreamReader(s, this.Encoding);
      {
        return reader.ReadToEnd();

      }
    }

    /// <summary>
    /// ��Xml��ʽ��ȡ���ص��ı�����
    /// </summary>
    /// <returns></returns>
    public XmlDocument GetXmlDocument()
    {
      using Stream s = this.HttpContent().ReadAsStream();
      StreamReader reader = new StreamReader(s, this.Encoding);
      XmlDocument doc = new XmlDocument();
      doc.Load(reader);
      return doc;
    }


    /// <summary>
    /// ��ȡXml��ʽ��ʾ��ʵ�����ʹ��XmlSerializer�����л�
    /// </summary>
    /// <param name="targetType">��Ҫ�����л��Ķ����C#��</param>
    /// <returns></returns>
    public object? GetXmlObject(Type targetType)
    {
      using Stream s = this.HttpContent().ReadAsStream();
      StreamReader reader = new StreamReader(s);
      XmlSerializer ser = new XmlSerializer(targetType);
      return ser.Deserialize(reader);
    }

    /// <summary>
    /// ��ȡJson��ʽ��ʾ��ʵ�����ʹ��Newtonsoft�����л�
    /// </summary>
    /// <param name="targetType">��Ҫ�����л��Ķ����C#��</param>
    /// <returns></returns>
    public object? GetJsonObject(Type targetType)
    {
      JsonSerializer serializer = new JsonSerializer();
      using Stream s = this.HttpContent().ReadAsStream();
      StreamReader sr = new StreamReader(s);
      JsonReader reader = new JsonTextReader(sr);
      return serializer.Deserialize(reader, targetType);
    }

    /// <summary>
    /// ��ȡ���ؽ�������ݶ���
    /// </summary>
    /// <returns></returns>
    public HttpContent HttpContent() { return this._response!.Content; }

    internal RequestResult(HttpResponseMessage response)
    {
      this._response = response;
      this.Encoding = Encoding.UTF8;
      this.StatusCode = response.StatusCode;

      if (!response.IsSuccessStatusCode)
      {
        ApiGatewayError error = CreateError(response);
        throw new MCTechOpenApiRequestException(error.Message, error);
      }
    }

    private static ApiGatewayError CreateError(HttpResponseMessage response)
    {
      XmlDocument document = new XmlDocument();
      document.Load(response.Content.ReadAsStream());
      XmlNodeList items = document.DocumentElement!.ChildNodes;
      Hashtable map = new Hashtable();
      foreach (XmlElement item in items)
      {
        string name = item.Name;
        string value = item.InnerText;
        map.Add(name, value);
      }

      return new ApiGatewayError(map);
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
