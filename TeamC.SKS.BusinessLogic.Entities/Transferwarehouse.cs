using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamC.SKS.BusinessLogic.Entities
{
    public class Transferwarehouse : Hop
    {
        public Geometry RegionGeometry { get; set; }

        public string LogisticsPartner { get; set; }

        public string LogisticsPartnerUrl { get; set; }
    }
}
