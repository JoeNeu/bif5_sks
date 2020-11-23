using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeamC.SKS.DataAccess.Entities
{
    public class Transferwarehouse : Hop
    {
        [Column(TypeName = "Geometry")]
        public Geometry RegionGeometry { get; set; }

        public string LogisticsPartner { get; set; }

        public string LogisticsPartnerUrl { get; set; }
    }
}
