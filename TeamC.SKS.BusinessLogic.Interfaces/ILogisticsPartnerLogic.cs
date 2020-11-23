using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Entities;

namespace TeamC.SKS.BusinessLogic.Interfaces
{
    public interface ILogisticsPartnerLogic
    {
        /// <summary>
        /// Transfer an existing parcel from the service of a logistics partner.
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6</param>
        public void TransferParcelPartner(string newtrackingId, string oldtrackingId);
    }
}
