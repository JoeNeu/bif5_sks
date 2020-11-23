using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.BusinessLogic.Entities.Validation;
using TeamC.SKS.BusinessLogic.Entities;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;
using DAL = TeamC.SKS.DataAccess.Entities;
using AutoMapper;
using TeamC.SKS.DataAccess.Interfaces;
using System.Linq;
using TeamC.SKS.ServiceAgents.Interfaces;
using Geocoding;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic
{
    public class SenderLogic : ISenderLogic
    {
        private readonly IParcelRepository _sqlRepoParcel;
        private readonly IHopRepository _sqlRepoHop;
        private readonly IMapper _mapper;
        private readonly IGeocoderAgent _agent;
        private readonly ILogger _logger;
        private static Random _random = new Random();

        public SenderLogic(IParcelRepository sqlRepoParcel, IHopRepository sqlRepoHop, IMapper mapper, IGeocoderAgent agent, ILogger<SenderLogic> logger)
        {
            _sqlRepoParcel = sqlRepoParcel;
            _sqlRepoHop = sqlRepoHop;
            _mapper = mapper;
            _agent = agent;
            _logger = logger;
            _logger.LogTrace("created SenderLogic");
        }
        public DTO.NewParcelInfo SubmitParcelIntoBL(Parcel parcel)
        {
            try
            {

                var validator = new ParcelValidation();
                var checkParcel = validator.Validate(parcel);

                if (!checkParcel.IsValid)
                {
                    throw new BLException("BL: Parcel Validation failed");
                }

                //check if parcel.trackingId is set -> only happens if it is from a Logistic Partner
                if (parcel.TrackingId == null || _sqlRepoParcel.GetByTrackingID(parcel.TrackingId) != null)
                {
                    //generate new trackingId until it is a unique
                    do
                    {
                        parcel.TrackingId = GenerateTrackingId();
                    } while (_sqlRepoParcel.GetByTrackingID(parcel.TrackingId) != null);
                }
                //Set State to InTransport
                parcel.State = Parcel.StateEnum.InTransportEnum;

                //Encode Sender and Receipient into a Location

                Location recLocation = _agent.EncodeGeocodeAsync(GenerateAddress(parcel.Receipient));
                Location sendLocation = _agent.EncodeGeocodeAsync(GenerateAddress(parcel.Sender));

                //Find responsible Truck for Sender - START
                //currently only for austria -> NO Transferwarehouse

                var senderTruck = _mapper.Map<Truck>(_sqlRepoHop.GetTruckByLocation(sendLocation));
                HopArrival firstHop = new HopArrival()
                {
                    Code = senderTruck.Code,
                    DateTime = DateTime.Now
                };
                parcel.VisitedHops.Add(firstHop);

                //Find responsible Truck for Recipient - FINISH
                var recTruck = _mapper.Map<Truck>(_sqlRepoHop.GetTruckByLocation(recLocation));

                //Calculate Route

                var rcptHopList = GetRouteToRoot(recTruck);
                var senderHopList = GetRouteToRoot(senderTruck);

                //var intersect = rcptHopList.Intersect(senderHopList,EqualityComparerFactory.Create<WarehouseNextHops>((a) => a.HopACode.GetHashCode(), (a, b) =))
                bool found = false;
                Hop intersection = null;
                foreach (var senderStep in senderHopList)
                {
                    if (found == false)
                    {
                        foreach (var step in rcptHopList)
                        {
                            if (senderStep.Code == step.Code)
                            {
                                intersection = senderStep;
                                found = true;
                                break;
                            }
                        }
                    }
                }
                //Get path to intersection
                var rcptJourney = rcptHopList.TakeWhile(p => p.Code != intersection.Code).ToList();
                var senderJourney = senderHopList.TakeWhile(p => p.Code != intersection.Code).ToList();
                senderJourney.Add(intersection);
                //reverse rcptJourney
                rcptJourney.Reverse();
                senderJourney.AddRange(rcptJourney);
                senderJourney.Add(recTruck);
                foreach (var step in senderJourney)
                {
                    HopArrival futureHop = new HopArrival()
                    {
                        Code = step.Code,
                        DateTime = DateTime.Now
                    };
                    parcel.FutureHops.Add(futureHop);
                }


                var parcelDAL = _mapper.Map<DAL.Parcel>(parcel);
                _sqlRepoParcel.Create(parcelDAL);

                var NPInfoMapped = _mapper.Map<DTO.NewParcelInfo>(parcel);
                DTO.NewParcelInfo NPInfo = (DTO.NewParcelInfo)NPInfoMapped;

                return NPInfo;
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

        private List<Hop> GetRouteToRoot(Hop first)
        {
            List<Hop> route = new List<Hop>();
            var child = _mapper.Map<DAL.Hop>(first);
            DAL.Hop parent = null;
            do
            {
                parent = _sqlRepoHop.GetParent(child);
                if (parent == null) break;
                child = parent;
                route.Add(_mapper.Map<Hop>(parent));
            } while (parent != null && parent.Description != "DEBUG");

            return route;
        }

        private string GenerateAddress(Receipient customer)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(customer.Street);
            strBuilder.Append(" ");
            strBuilder.Append(customer.PostalCode);
            strBuilder.Append(" ");
            strBuilder.Append(customer.Country);
            return strBuilder.ToString();
        }

        private string GenerateTrackingId()
        {
            int length = 9;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
