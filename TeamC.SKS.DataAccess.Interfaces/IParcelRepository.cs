using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.DataAccess.Interfaces
{
    public interface IParcelRepository
    {
        int Create(Parcel p);
        void Update(Parcel p);
        void Delete(int ID);
        Parcel GetByID(int ID);
        Parcel GetByTrackingID(string trackingID);
        IEnumerable<Parcel> GetByName(string name);
    }
}
