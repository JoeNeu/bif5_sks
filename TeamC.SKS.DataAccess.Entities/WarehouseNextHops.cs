using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeamC.SKS.DataAccess.Entities
{
    public class WarehouseNextHops
    {

        public string HopACode { get; set; }
        public string HopBCode { get; set; }

        public Warehouse HopA { get; set; }
        public Hop HopB { get; set; }

        public decimal? TraveltimeMins { get; set; }
    }
}
