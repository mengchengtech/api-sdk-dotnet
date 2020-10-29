using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    public class MCTechException : ApplicationException
    {
        public MCTechException(string message)
            : base(message)
        {

        }
    }
}
