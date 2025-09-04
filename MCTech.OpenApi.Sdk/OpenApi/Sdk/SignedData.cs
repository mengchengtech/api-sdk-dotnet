using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class SignedData
  {
    public string Signable { get; private set; }
    public string Signature { get; private set; }

    public SignedData(string signable, string signature)
    {
      this.Signable = signable;
      this.Signature = signature;
    }
  }
}