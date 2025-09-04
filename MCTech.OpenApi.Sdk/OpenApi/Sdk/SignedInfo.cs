using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class SignedInfo
  {
    public SignatureMode Mode { get; private set; }
    public SignedData Signed { get; private set; }
    public IDictionary<string, string>? Headers { get; private set; }
    public IDictionary<string, string>? Query { get; private set; }

    public SignedInfo(SignatureMode mode, SignedData signed, IDictionary<string, string>? headers, IDictionary<string, string>? query)
    {
      this.Mode = mode;
      this.Signed = signed;
      this.Headers = headers;
      this.Query = query;
    }
  }
}