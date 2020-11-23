using System.Collections.Generic;

namespace TeamC.SKS.DataAccess.Entities
{
    public class Warehouse : Hop
    {
        public IEnumerable<WarehouseNextHops> NextHops { get; set; }
    }
}
