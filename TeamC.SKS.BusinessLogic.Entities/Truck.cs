using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NetTopologySuite.Geometries;

namespace TeamC.SKS.BusinessLogic.Entities
{ 
    public class Truck : Hop
    {
        public Geometry RegionGeometry { get; set; }

        public string NumberPlate { get; set; }
    }
}
