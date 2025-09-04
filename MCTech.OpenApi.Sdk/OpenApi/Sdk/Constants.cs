using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class Constants
  {
    public const string QUERY_ACCESS_ID = "AccessId";
    public const string QUERY_EXPIRES = "Expires";
    public const string QUERY_SIGNATURE = "Signature";
    public static readonly string[] QUERY_KEYS = ["AccessId", "Signature", "Expires"];
    public const string CUSTOM_PREFIX = "x-iwop-";
    /**
     * 生成Query签名时间有效期默认值，单位秒
     */
    public const long DEFAULT_EXPIRES = 30;
  }
}