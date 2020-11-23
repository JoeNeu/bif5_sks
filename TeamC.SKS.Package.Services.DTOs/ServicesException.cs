using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.Package.Services.DTOs.Models
{
    public class ServicesException : Exception
    {
        public ServicesException(string message) : base(message)
        {
        }

        public ServicesException(string message, Exception innerLayer) : base(message, innerLayer)
        {
        }
    }
}
