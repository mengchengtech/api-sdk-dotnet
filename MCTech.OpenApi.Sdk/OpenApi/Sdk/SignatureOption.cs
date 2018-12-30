using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    internal class SignatureOption
    {
        private readonly Uri _requestUri;
        private readonly string _method;
        private readonly string _contentType;
        private readonly DateTime _date;
        //private readonly string _contentMd5;
            
        public SignatureOption(Uri requestUri, string method, string contentType, DateTime date)
        {
            if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch) {
                if (string.IsNullOrWhiteSpace(contentType)) {
                    throw new MCTechOpenApiException("http请求缺少'content-type'头。请求方式为[" + method + "]时，需要在RpcInvoker的headers属性上设置'content-type'");
                }
            }
            
            this._requestUri = requestUri;
            this._method = method;
            this._contentType = contentType;
            //this._contentMd5 = "";
            this._date = date;
            this.Headers = new NameValueCollection();
        }

        /// <summary>
        /// 获取或设置设置REST调用签名中的method信息
        /// </summary>
        public string Method
        {
            get{ return this._method; }
        }

        /// <summary>
        /// 获取或设置REST调用中的content-type头
        /// </summary>
        public string ContentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType))
                {
                    return string.Empty;
                }
                return _contentType;
            }
        }

        //public string ContentMd5 
        //{ 
        //    get { return this._contentMd5; } 
        //}
        
        /// <summary>
        /// 获取或设置REST调用签名中的url路径信息
        /// </summary>
        public Uri resourceUri
        {
            get { return this._requestUri; }
        }

        /// <summary>
        /// 发出请求的客户端时间
        /// </summary>
        public DateTime Date 
        {
            get { return _date; }
        }

        /// <summary>
        /// 自定义的headers头
        /// </summary>
        public NameValueCollection Headers { get; set; }
    }
}
