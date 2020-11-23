using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.BusinessLogic.Entities.Validation;
using TeamC.SKS.BusinessLogic.Entities;
using Newtonsoft.Json;
using TeamC.SKS.DataAccess.Interfaces;
using DAL = TeamC.SKS.DataAccess.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic
{
    public class WarehouseManagementLogic : IWarehouseManagementLogic
    {
        private readonly IHopRepository _sqlRepo;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public WarehouseManagementLogic(IHopRepository sqlRepo, IMapper mapper, ILogger<WarehouseManagementLogic> logger)
        {
            _sqlRepo = sqlRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogTrace("created WarehouseManagementLogic");
        }
        public Warehouse ExportHierarchy()
        {
            try
            {
                var root = _sqlRepo.GetWarehouseRoot();
                return _mapper.Map<Warehouse>(root);
            }
            catch (DAL.DALException exc)
            {
                _logger.LogError(exc.ToString());
                throw new BLException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                throw new BLException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public void ImportHierarchy(Warehouse _wh)
        {
            var validator = new WarehouseValidation();
            var checkWarehouse = validator.Validate(_wh);

            if (!checkWarehouse.IsValid)
            {
                throw new Exception("Import Warehouse Tree failed in BL");
            }
            var whDAL = _mapper.Map<DAL.Warehouse>(_wh);
            try
            {
                _sqlRepo.ClearDatabase();
                _sqlRepo.Create(whDAL);
            }
            catch (DAL.DALException exc)
            {
                _logger.LogError(exc.ToString());
                throw new BLException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                throw new BLException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }
    }
}
