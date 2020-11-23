using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Interfaces;
using FluentValidation;
using TeamC.SKS.BusinessLogic.Entities.Validation;
using TeamC.SKS.BusinessLogic.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;
using AutoMapper;
using TeamC.SKS.DataAccess.Interfaces;
using DAL = TeamC.SKS.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic
{
    public class LogisticsPartnerLogic : ILogisticsPartnerLogic
    {
        private readonly IParcelRepository _sqlRepo;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public LogisticsPartnerLogic(IParcelRepository sqlRepo, IMapper mapper, ILogger<LogisticsPartnerLogic> logger)
        {
            _sqlRepo = sqlRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogTrace("created LogisticsPartnerLogic");
        }
        public void TransferParcelPartner(string newtrackingId, string oldtrackingId)
        {
            try
            {
                DAL.Parcel p = _sqlRepo.GetByTrackingID(newtrackingId);
                p.OldTrackingId = oldtrackingId;
                _sqlRepo.Update(p);
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
