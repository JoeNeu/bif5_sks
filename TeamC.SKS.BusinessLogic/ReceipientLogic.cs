using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.BusinessLogic.Entities.Validation;
using TeamC.SKS.BusinessLogic.Entities;
using Newtonsoft.Json;
using AutoMapper;
using TeamC.SKS.DataAccess.Interfaces;
using DAL = TeamC.SKS.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic
{
    public class ReceipientLogic : IReceipientLogic
    {
        private readonly IParcelRepository _sqlRepo;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReceipientLogic(IParcelRepository sqlRepo, IMapper mapper, ILogger<ReceipientLogic> logger)
        {
            _sqlRepo = sqlRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogTrace("created RecipientLogic");
        }
        public Parcel TrackPackage(string trackingId)
        {
            try
            {
                DAL.Parcel p = _sqlRepo.GetByTrackingID(trackingId);
                return _mapper.Map<Parcel>(p);
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
