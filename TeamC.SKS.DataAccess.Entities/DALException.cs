using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.DataAccess.Entities
{
    public class DALException : Exception
    {
        public DALException(string message) : base(message)
        {
        }

        public DALException(string message, Exception innerLayer) : base(message, innerLayer)
        {
        }
    }
}
