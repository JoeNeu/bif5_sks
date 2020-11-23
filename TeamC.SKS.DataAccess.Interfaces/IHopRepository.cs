using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.DataAccess.Interfaces
{
    public interface IHopRepository
    {
        string Create(Hop hop);
        void Update(Hop hop);
        void Delete(string id);
        Hop GetByHopType(string hopType);
        Hop GetByCode(string code);
        Warehouse GetWarehouseRoot();
        Truck GetTruckByLocation(Geocoding.Location location);
        void ClearDatabase();
        Hop GetParent(Hop hop);
    }
}
