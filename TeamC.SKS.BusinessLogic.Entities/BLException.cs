using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.BusinessLogic.Entities
{
    public class BLException : Exception
    {
        public BLException(string message) : base(message)
        {
        }

        public BLException(string message, Exception innerLayer) : base(message, innerLayer)
        {
        }
    }
}
