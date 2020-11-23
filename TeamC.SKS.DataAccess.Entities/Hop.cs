using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeamC.SKS.DataAccess.Entities
{
    public class Hop
    {

        [Required, Key]
        public string Code { get; set; }
        public decimal? ProcessingDelayMins { get; set; }
        public string Description { get; set; }
        [Required]
        public string HopType { get; set; }

        public WarehouseNextHops InboundTransport { get; set; }
    }
}
