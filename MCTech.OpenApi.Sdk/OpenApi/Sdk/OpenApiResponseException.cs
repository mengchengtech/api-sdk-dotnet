using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
  public class OpenApiResponseException : Exception
  {
    public ApiGatewayErrorData Error { get; private set; }
    public HttpStatusCode Status { get; private set; }

    public OpenApiResponseException(string message, HttpStatusCode status, ApiGatewayErrorData error)
    : this(message, status, error, null)
    {

    }

    public OpenApiResponseException(string message, HttpStatusCode status, ApiGatewayErrorData error, Exception? innerException)
    : base(message, innerException)
    {
      this.Error = error;
      this.Status = status;
    }
  }
}
