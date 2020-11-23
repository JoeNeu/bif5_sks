using System;
using System.Collections.Generic;
using System.Text;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;

namespace TeamC.SKS.BusinessLogic.Interfaces
{
    public interface IStaffLogic
    {
        /// <summary>
        /// Report that a Parcel has been delivered at it's final destination address.
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6</param>
        public bool ReportDeliveryFinal(string trackingId);
        //public bool ReportDeliveryFinal(DTO.NewParcelInfo ParcelInfo);
        /// <summary>
        /// Report that a Parcel has arrived at a certain hop either Warehouse or Truck.
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6</param>
        /// <param name="code">The Code of the hop (Warehouse or Truck).</param>
        public bool StaffReportHop(string trackingId, string code);
    }
}
