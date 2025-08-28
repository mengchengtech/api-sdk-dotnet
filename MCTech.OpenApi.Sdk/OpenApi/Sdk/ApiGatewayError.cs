using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
  public class ApiGatewayError
  {
    private const string PROP_CODE = "Code";
    private const string PROP_MESSAGE = "Message";
    private const string PROP_STRING_TO_SIGN_BYTES = "StringToSignBytes";
    private const string PROP_SIGNATURE_PROVIDED = "SignatureProvided";
    private const string PROP_STRING_TO_SIGN = "StringToSign";
    private const string PROP_ACCESS_KEY_ID = "AccessKeyId";

    private readonly Hashtable _map;

    public ApiGatewayError(Hashtable map)
    {
      this._map = map;
    }

    public string Code
    {
      get { return (string)this._map[PROP_CODE]!; }
    }

    public string Message
    {
      get { return (string)this._map[PROP_MESSAGE]!; }
    }

    public string? StringToSignBytes
    {
      get { return (string?)this._map[PROP_STRING_TO_SIGN_BYTES]; }
    }

    public string? SignatureProvided
    {
      get { return (string?)this._map[PROP_SIGNATURE_PROVIDED]; }
    }

    public string? StringToSign
    {
      get { return (string?)this._map[PROP_STRING_TO_SIGN]; }
    }

    public string? AccessKeyId
    {
      get { return (string?)this._map[PROP_ACCESS_KEY_ID]; }
    }

    public string? GetProperty(String name)
    {
      return (string?)this._map[name];
    }
  }
}
