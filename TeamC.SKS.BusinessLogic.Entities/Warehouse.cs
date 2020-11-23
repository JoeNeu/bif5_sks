using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TeamC.SKS.BusinessLogic.Entities
{ 
    public partial class Warehouse : Hop
    {
        public IEnumerable<WarehouseNextHops> NextHops { get; set; }
    }
}
