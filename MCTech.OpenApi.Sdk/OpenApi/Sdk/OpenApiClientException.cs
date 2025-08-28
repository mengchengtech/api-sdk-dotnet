using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
  public class OpenApiClientException : ApplicationException
  {

    public OpenApiClientException(string message)
        : base(message)
    {
    }
  }
}
