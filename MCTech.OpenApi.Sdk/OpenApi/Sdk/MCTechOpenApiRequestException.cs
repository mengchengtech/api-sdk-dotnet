using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTech.OpenApi.Sdk
{
    public class MCTechOpenApiRequestException : MCTechException
    {
        private readonly ApiGatewayError error;

        public MCTechOpenApiRequestException(string message, ApiGatewayError error) 
        :
            base(message){
            this.error = error;
        }
    }
}
