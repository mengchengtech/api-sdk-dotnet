using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
  public class OpenApiResponseException : Exception
  {
    public ApiGatewayErrorData Error { get; private set; }

    public OpenApiResponseException(string message, ApiGatewayErrorData error)
    : base(message)
    {
      this.Error = error;
    }
  }
}
