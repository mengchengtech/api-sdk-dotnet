using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    public class ApiGatewayError {
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

    public string getCode()
    {
        return (string)this._map[PROP_CODE];
    }

    public string getMessage()
    {
        return (string)this._map[PROP_MESSAGE];
    }

    public string getStringToSignBytes()
    {
        return (string)this._map[PROP_STRING_TO_SIGN_BYTES];
    }

    public string getSignatureProvided()
    {
        return (string)this._map[PROP_SIGNATURE_PROVIDED];
    }

    public string getStringToSign() 
    {
        return (string)this._map[PROP_STRING_TO_SIGN];
    }

    public string getAccessKeyId() 
    {
        return (string)this._map[PROP_ACCESS_KEY_ID];
    }

    public string getProperty(String name) {
        return (string)this._map[name];
    }
}
}
