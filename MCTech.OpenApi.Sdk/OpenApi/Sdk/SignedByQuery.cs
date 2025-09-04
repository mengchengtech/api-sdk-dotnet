using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class SignedByQuery : ISignedBy
  {
    public SignatureMode Mode
    {
      get { return SignatureMode.Query; }
    }

    public QuerySignatureParams? Parameters { get; private set; }

    public SignedByQuery()
    {
    }

    public SignedByQuery(QuerySignatureParams? parameters)
    {
      this.Parameters = parameters;
    }
  }
}