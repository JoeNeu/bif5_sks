using AutoMapper;
using Geocoding;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TeamC.SKS.BusinessLogic;
using TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.Package.Services.Mapper;
using TeamC.SKS.ServiceAgents;
using TeamC.SKS.ServiceAgents.Interfaces;
using DAL = TeamC.SKS.DataAccess.Entities;
using DTO = TeamC.SKS.Package.Services.DTOs.Models;

namespace TeamC.SKS.BusinessLogic.Tests
{
    [TestFixture]
    public class SenderLogicTest
    {
        IMapper _mapper;
        Mock<IParcelRepository> _mockParcel;
        Mock<IHopRepository> _mockHop;
        Receipient customer;
        DAL.Receipient DALcustomer;
        Parcel parcel;
        DAL.Parcel DALparcel;
        DAL.Truck DALtruck;
        DAL.Warehouse roothop;
        Mock<IGeocoderAgent> _agent;
        private SenderLogic _controller;
        public SenderLogicTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mockParcel = new Mock<IParcelRepository>();
            _mockHop = new Mock<IHopRepository>();
            _agent = new Mock<IGeocoderAgent>();
            _controller = new SenderLogic(_mockParcel.Object, _mockHop.Object, _mapper, _agent.Object, new LoggerFactory().CreateLogger<SenderLogic>());

            customer = new Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"

            };
            DALcustomer = new DAL.Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            parcel = new Parcel()
            {
                Weight = 1,
                State = Parcel.StateEnum.InTransportEnum,
                Receipient = customer,
                Sender = customer,
                TrackingId = "ABCDEF123"
            };
            DALparcel = new DAL.Parcel()
            {
                Weight = 1,
                State = DAL.Parcel.StateEnum.InTransportEnum,
                Receipient = DALcustomer,
                Sender = DALcustomer,
                TrackingId = "ABCDEF123"
            };
            DALtruck = new DAL.Truck()
            {
                Code = "AUTA05",
                NumberPlate = "lul"
            };
            roothop = new DAL.Warehouse()
            {
                Code = "AUTA05",
                Description = "DEBUG",
                HopType = "Warehouse",
                NextHops = new List<DAL.WarehouseNextHops>()
                {
                    new DAL.WarehouseNextHops()
                    {
                        HopACode = "AUTA05",
                        HopBCode = "BNDA04",
                        HopB = new DAL.Warehouse()
                        {
                            Code = "BNDA04"
                        }
                    }
                }
            };
        }
        [Test]
        public void SubmitParcelIntoBL_ShouldReturnNewParcelInfo_WhenSucceeds()
        {
            string trackingId = "ABCDEF123";
            Location recLocation = new Location(longitude: 2, latitude: 3);
            parcel.TrackingId = trackingId;
            //1
            _mockParcel.Setup(x => x.GetByTrackingID(trackingId)).Returns(DALparcel);
            //2 Encode
            _agent.Setup(x => x.EncodeGeocodeAsync(trackingId)).Returns(recLocation);
            //3 location
            _mockHop.Setup(x => x.GetTruckByLocation(It.IsAny<Location>())).Returns(DALtruck);
            //4 create
            _mockParcel.Setup(x => x.Create(It.IsAny<DAL.Parcel>())).Returns(1);
            //5 get route to root 
            _mockHop.Setup(x => x.GetParent(It.IsAny<DAL.Hop>())).Returns(roothop);

            var result = _controller.SubmitParcelIntoBL(parcel);

            Assert.IsInstanceOf<DTO.NewParcelInfo>(result);
        }
        [Test]
        public void SubmitParcelIntoBL_ShouldReturnThrowExeption_WhenValidationFailed()
        {
            parcel.TrackingId = "X";

            BLException exc = Assert.Throws<BLException>(() => _controller.SubmitParcelIntoBL(parcel));

            Assert.AreEqual($"BL: Parcel Validation failed", exc.InnerException.Message);
        }
        //[Test]
        //public void SubmitParcelIntoBL_ShouldReturnThrowExeption_WhenDALSUBMITFailedInBL()
        //{
        //    string trackingId = "ABCDEF123";
        //    _mockParcel.Setup(x => x.GetByTrackingID(trackingId)).Throws(new Exception("Err"));
        //    BLException exc = Assert.Throws<BLException>(() => _controller.SubmitParcelIntoBL(parcel));
        //    StringAssert.StartsWith("Err", exc.InnerException.Message);
        //}
        //[Test]
        //public void SubmitParcelIntoBL_ShouldReturnThrowExeption_WhenDALSUBMITFailedInDAL()
        //{
        //    string trackingId = "ABCDEF123";
        //    _mockParcel.Setup(x => x.GetByTrackingID(trackingId)).Throws(new DAL.DALException("Err"));
        //    BLException exc = Assert.Throws<BLException>(() => _controller.SubmitParcelIntoBL(parcel));
        //    StringAssert.StartsWith("Err", exc.InnerException.Message);
        //}
    }
}
