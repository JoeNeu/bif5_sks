using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Entities;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;

namespace TeamC.SKS.BusinessLogic.Interfaces
{
    public interface ISenderLogic
    {
        /// <summary>
        /// /parcel
        /// Submit a new parcel to the logistics service.
        /// </summary>
        public DTO.NewParcelInfo SubmitParcelIntoBL(Parcel parcel);
    }
}
