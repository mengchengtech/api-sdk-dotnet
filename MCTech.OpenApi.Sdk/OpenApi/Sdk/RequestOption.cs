using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace MCTech.OpenApi.Sdk
{
  public class RequestOption
  {
    public ISignedBy? SignedBy { get; private set; }
    public int? Timeout { get; private set; }
    public IDictionary<string, string?> Query { get; private set; }
    public IDictionary<string, string?> Headers { get; private set; }
    public string? ContentType
    {
      get { return this.Entity?.Headers.ContentType?.ToString(); }
    }

    public HttpContent? Entity { get; private set; }

    private RequestOption(ISignedBy? signedBy, int? timeout, IDictionary<string, string?> query, IDictionary<string, string?> headers, HttpContent? entity)
    {
      this.SignedBy = signedBy;
      this.Timeout = timeout;
      this.Query = query;
      this.Headers = headers;
      this.Entity = entity;
    }

    public static Builder NewBuilder()
    {
      return new Builder();
    }

    public class Builder
    {
      private ISignedBy? signedBy;
      private int? timeout;
      private readonly IDictionary<string, string?> query;
      private readonly IDictionary<string, string?> headers;
      private string? contentType;
      private object? body;


      internal Builder()
      {
        this.query = new Dictionary<string, string?>();
        this.headers = new Dictionary<string, string?>();
      }

      public Builder SignedBy(ISignedBy signedBy)
      {
        this.signedBy = signedBy;
        return this;
      }

      public Builder Timeout(int timeout)
      {
        this.timeout = timeout;
        return this;
      }

      public Builder AddQuery(string name, object value)
      {
        this.query.Add(name, value.ToString());
        return this;
      }

      public Builder AddQuery(IDictionary<string, object> query)
      {
        if (query != null && query.Count > 0)
        {
          foreach (var (name, value) in query)
          {
            this.query.Add(name, value.ToString());
          }
        }
        return this;
      }

      public Builder AddHeader(string name, object value)
      {
        this.headers.Add(name, value.ToString());
        return this;
      }

      public Builder AddHeader(IDictionary<string, object> headers)
      {
        if (headers != null && headers.Count > 0)
        {
          foreach (var (name, value) in headers)
          {
            this.headers.Add(name, value.ToString());
          }
        }
        return this;
      }

      public Builder ContentType(string contentType)
      {
        this.contentType = contentType;
        return this;
      }

      public Builder Body(string body)
      {
        this.body = body;
        return this;
      }

      public Builder Body(Stream body)
      {
        this.body = body;
        return this;
      }

      public RequestOption Build()
      {
        HttpContent? entity = null;
        if (this.body != null)
        {
          if (this.body is string)
          {
            entity = new StringContent((string)this.body);
          }
          else if (this.body is Stream)
          {
            entity = new StreamContent((Stream)this.body);
          }
          else
          {
            throw new NotSupportedException("不支持的body类型。当前仅支持string和Stream");
          }

          if (!string.IsNullOrEmpty(this.contentType))
          {
            entity.Headers.ContentType = new MediaTypeHeaderValue(this.contentType);
          }
        }
        return new RequestOption(signedBy, timeout, query, headers, entity);
      }
    }
  }
}