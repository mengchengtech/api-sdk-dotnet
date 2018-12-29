using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTech.OpenApi.Sdk
{
    public class MctechException : ApplicationException
    {
        public MctechException(string message)
            : base(message)
        {

        }
    }
}
