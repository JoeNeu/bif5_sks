using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Entities;

namespace TeamC.SKS.BusinessLogic.Interfaces
{
    public interface IReceipientLogic
    {
        /// <summary>
        /// Find the latest tracking state of a parcel by its tracking ID.
        /// /parcel/{trackingId}
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6</param>
        public Parcel TrackPackage(string trackingId);
    }
}
