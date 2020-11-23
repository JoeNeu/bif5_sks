using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.BusinessLogic.Entities
{
    public class WarehouseNextHops
    {
        public decimal? TraveltimeMins { get; set; }

        public Hop Hop { get; set; }
    }
}
