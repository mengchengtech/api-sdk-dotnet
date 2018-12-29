using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    public class MctechOpenApiException : MctechException
    {

        public MctechOpenApiException(string message)
            : base(message)
        {
        }
    }
}
