using System;
using System.Text;
using System.Collections.Specialized;

namespace MCTech.OpenApi.Sdk
{
  public class QuerySignatureParams
  {
    public int Duration { get; private set; }


    public QuerySignatureParams(int duration)
    {
      this.Duration = duration;
    }
  }
}