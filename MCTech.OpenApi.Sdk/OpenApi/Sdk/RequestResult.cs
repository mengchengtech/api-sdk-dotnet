using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MCTech.OpenApi.Sdk
{
    public class RequestResult : IDisposable
    {
        private HttpWebResponse _response;
        /// <summary>
        /// 获取或设置返回的响应内容的编码
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// 返回结果状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 以字符串方式获取返回的文本内容
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            using (Stream s = this.OpenRead())
            {
                StreamReader reader = new StreamReader(s, this.Encoding);
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 以Xml方式获取返回的文本内容
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {
            using (Stream s = this.OpenRead())
            {
                StreamReader reader = new StreamReader(s, this.Encoding);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return doc;
            }
        }


        /// <summary>
        /// 获取Xml方式表示的实体对象。使用XmlSerializer反序列化
        /// </summary>
        /// <param name="ignoreError">是否忽略Http状态异常</param>
        /// <param name="targetType">需要返序列化的对象的C#类</param>
        /// <returns></returns>
        public object GetXmlObject(Type targetType)
        {
            using (Stream s = OpenRead())
            {
                StreamReader reader = new StreamReader(s);
                XmlSerializer ser = new XmlSerializer(targetType);
                return ser.Deserialize(reader);
            }
        }

        /// <summary>
        /// 获取Json方式表示的实体对象。使用Newtonsoft反序列化
        /// </summary>
        /// <param name="targetType">需要返序列化的对象的C#类</param>
        /// <returns></returns>
        public object GetJsonObject(Type targetType)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (Stream s = OpenRead())
            {
                StreamReader sr = new StreamReader(s);
                JsonReader reader = new JsonTextReader(sr);
                return serializer.Deserialize(reader, targetType);
            }
        }

        /// <summary>
        /// 获取一个用于读返回结果的流
        /// </summary>
        /// <returns></returns>
        public Stream OpenRead()
        {
            return this._response.GetResponseStream();
        }

        private string _contentType;
        /// <summary>
        /// 获取内容的ContentType
        /// </summary>
        public string ContentType
        {
            get{ return _contentType; }
        }

        internal RequestResult(HttpWebResponse response)
        {
            _response = response;
            _contentType = response.ContentType;
            this.Encoding = Encoding.UTF8;
            this.StatusCode = response.StatusCode;

            if (this.StatusCode >= HttpStatusCode.BadRequest)
            {
                ApiGatewayError error = CreateError(response);
                throw new MCTechOpenApiRequestException(error.getMessage(), error);
            }
        }

        private static ApiGatewayError CreateError(HttpWebResponse response) {
            XmlDocument document = new XmlDocument();
            document.Load(response.GetResponseStream());
            XmlNodeList items = document.DocumentElement.ChildNodes;
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
                if (_response != null)
                {
                    _response.Close();
                    _response = null;
                }
            }
        }

        ~RequestResult()
        {
            Dispose(false);
        }
    }
}
