using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.BusinessLogic.Entities
{
    public class Hop
    {
        public string Code { get; set; }

        public decimal? ProcessingDelayMins { get; set; }

        public string Description { get; set; }

        public string HopType { get; set; }
    }
}
