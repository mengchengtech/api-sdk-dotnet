using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    public class MCTechOpenApiException : MCTechException
    {

        public MCTechOpenApiException(string message)
            : base(message)
        {
        }
    }
}
