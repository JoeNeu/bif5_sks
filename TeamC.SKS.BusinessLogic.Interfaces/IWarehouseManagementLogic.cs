using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Entities;

namespace TeamC.SKS.BusinessLogic.Interfaces
{
    public interface IWarehouseManagementLogic
    {
        /// <summary>
        /// Exports the hierarchy of Warehouse and Truck objects.
        /// </summary>
        public Warehouse ExportHierarchy();
        /// <summary>
        /// Imports a hierarchy of Warehouse and Truck objects.
        /// </summary>
        public void ImportHierarchy(Warehouse Body);
    }
}
