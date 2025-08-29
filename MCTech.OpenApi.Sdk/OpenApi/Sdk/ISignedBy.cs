using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public interface ISignedBy
  {
    SignatureMode Mode { get; }
  }
}