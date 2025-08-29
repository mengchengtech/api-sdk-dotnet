using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class SignedByHeader : ISignedBy
  {
    public SignatureMode Mode
    {
      get { return SignatureMode.Header; }
    }
  }
}