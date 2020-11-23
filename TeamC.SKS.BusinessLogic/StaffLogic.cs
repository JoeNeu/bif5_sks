using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.BusinessLogic.Entities.Validation;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.DataAccess.Interfaces;
using DAL = TeamC.SKS.DataAccess.Entities;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace TeamC.SKS.BusinessLogic
{
    public class StaffLogic : IStaffLogic
    {
        private readonly IParcelRepository _sqlRepoParcel;
        private readonly IHopRepository _sqlRepoHop;
        private readonly IWebhookRepository _webrep;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public StaffLogic(IParcelRepository sqlRepoParcel,IHopRepository sqlRepoHop,IWebhookRepository webrep, IMapper mapper, ILogger<StaffLogic> logger, HttpClient client)
        {
            _sqlRepoParcel = sqlRepoParcel;
            _sqlRepoHop = sqlRepoHop;
            _webrep = webrep;
            _mapper = mapper;
            _logger = logger;
            _client = client;
            _logger.LogTrace("created StaffLogic");
        }
        public bool ReportDeliveryFinal(string trackingId)
        {
            try
            {
                var parcelDal = _sqlRepoParcel.GetByTrackingID(trackingId);
                if (parcelDal == null || parcelDal.FutureHops.Count != 1)
                {
                    return false;
                }
                var parcelBL = _mapper.Map<Parcel>(parcelDal);
                var validator = new ParcelValidation();
                var checkParcel = validator.Validate(parcelBL);

                if (!checkParcel.IsValid)
                {
                    return false;
                }
                parcelDal.FutureHops.RemoveAt(0);
                parcelDal.State = DAL.Parcel.StateEnum.DeliveredEnum;
                _sqlRepoParcel.Update(parcelDal);
                //contact Webhook Subscriber
                //Find Webhook IDs with the same trackingID
                List<DAL.Webhook> deleteList = _webrep.GetWebhooksByTrackingID(trackingId);

                foreach(var hook in deleteList)
                {
                    _webrep.Delete(hook.Id);
                }
                return true;
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

        public bool StaffReportHop(string trackingId, string code)
        {
            try
            {
                //Get Data from DB and check if exists
                var parcelDal = _sqlRepoParcel.GetByTrackingID(trackingId);
                var hopDal = _sqlRepoHop.GetByCode(code);
                if (parcelDal == null || hopDal == null || parcelDal.FutureHops.Count == 1 || parcelDal.FutureHops[0].Code != code)
                {
                    return false;
                }
                //Validation
                var parcelBL = _mapper.Map<Parcel>(parcelDal);
                var hopBL = new HopArrival()
                {
                    Code = code,
                    DateTime = DateTime.Now
                };
                var validatorParcel = new ParcelValidation();
                var validatorHopArrival = new HopArrivalValidation();
                var checkParcel = validatorParcel.Validate(parcelBL);
                var checkHop = validatorHopArrival.Validate(hopBL);
                //check if Parcel and Hop is Valid and the next Hop is the same as the incoming Hop
                if (!checkParcel.IsValid && !checkHop.IsValid)
                {
                    return false;
                }

                //Change State
                var hopArrival = _mapper.Map<DAL.HopArrival>(hopBL);
                if (hopDal.HopType == "Truck")
                {
                    parcelDal.State = DAL.Parcel.StateEnum.InTruckDeliveryEnum;
                }
                if (hopDal.HopType == "Transferwarehouse")
                {
                    DAL.Transferwarehouse post = (DAL.Transferwarehouse)hopDal;

                    string url = $"{post.LogisticsPartnerUrl}/parcel/{trackingId}";
                    HttpResponseMessage msg = SendParcelToPartner(url, parcelBL);
                    
                    if(msg.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Could not POST Parcel to Partner {post.LogisticsPartnerUrl}");
                    }
                    parcelDal.State = DAL.Parcel.StateEnum.DeliveredEnum;
                }
                if (hopDal.HopType == "Warehouse")
                {
                    parcelDal.State = DAL.Parcel.StateEnum.InTransportEnum;
                }
                // Update Visited/Future Hops
                parcelDal.FutureHops.RemoveAt(0);
                parcelDal.VisitedHops.Add(hopArrival);
                _sqlRepoParcel.Update(parcelDal);

                //Contact Webhook Subscriber
                List<DAL.Webhook> contactList = _webrep.GetWebhooksByTrackingID(trackingId);
                var webhookparcel = _mapper.Map<Parcel>(parcelDal);
                foreach (var hook in contactList)
                {
                    HttpResponseMessage msg = SendWebhookResponse(hook.Url, webhookparcel);
                }
                return true;
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

        private HttpResponseMessage SendParcelToPartner(string url, Parcel parcel)
        {
            var content = _mapper.Map<DTO.Parcel>(parcel);
            var json = JsonConvert.SerializeObject(content);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            return _client.PostAsync(url, body).Result;
        }
        private HttpResponseMessage SendWebhookResponse(string url, Parcel parcel)
        {
            //create WebhookMessage
            var content = _mapper.Map<DTO.WebhookMessage>(parcel);
            
            var json = JsonConvert.SerializeObject(content);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            return _client.PostAsync(url, body).Result;
        }
    }
}
