using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.Package.Services.Mapper;
using DAL = TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.BusinessLogic.Tests
{
    [TestFixture]
    public class StaffLogicTest
    {
        IMapper _mapper;
        Mock<IParcelRepository> _mock;
        Mock<IHopRepository> _mockHOP;
        Mock<IWebhookRepository> _mockHook;
        private StaffLogic _controller;
        HttpClient _client;

        public StaffLogicTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IParcelRepository>();
            _mockHOP = new Mock<IHopRepository>();
            _mockHook = new Mock<IWebhookRepository>();
            _client = new HttpClient();
            _controller = new StaffLogic(_mock.Object, _mockHOP.Object, _mockHook.Object, _mapper, new LoggerFactory().CreateLogger<StaffLogic>(), _client);

        }
        [Test]
        public void ReportDeliveryFinal_ShouldReturnTrue_WhenSucceeds()
        {
            string trackingId = "PYJRB4HZ6";
            DAL.Receipient customer = new DAL.Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            var hop = new DAL.HopArrival()
            {
                Code = "BNDA01"
            };

            DAL.Parcel DALp = new DAL.Parcel()
            {
                TrackingId = trackingId,
                Receipient = customer,
                Sender = customer
            };

            DALp.FutureHops.Add(hop);
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(DALp);
            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            List<DAL.Webhook> deleteList = new List<DAL.Webhook>();
            _mockHook.Setup(x => x.GetWebhooksByTrackingID(trackingId)).Returns(deleteList);
            // Act
            var result = _controller.ReportDeliveryFinal(trackingId);

            // Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void ReportDeliveryFinal_ShouldReturnFalse_WhenNotSucceeds()
        {
            string trackingId = "FALSCH";
            DAL.Parcel p = null;
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(p);
            // Act
            var result = _controller.ReportDeliveryFinal(trackingId);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void ReportDeliveryFinal_ShouldReturnDALExceptopn_WhenNotFailsInDAL()
        {
            string trackingId = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new DAL.DALException("Err"));
            BLException exc = Assert.Throws<BLException>(() => _controller.ReportDeliveryFinal(trackingId));
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
        [Test]
        public void ReportDeliveryFinal_ShouldReturnDALExceptopn_WhenNotFailsInBL()
        {
            string trackingId = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new Exception("Err"));
            BLException exc = Assert.Throws<BLException>(() => _controller.ReportDeliveryFinal(trackingId));
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
        [Test]
        public void ReportDeliveryFinal_ShouldReturnFalse_WhenNotValid()
        {
            string trackingId = "PYJRB4HZ6";
            DAL.Receipient customer = new DAL.Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "FALSCH"
            };
            var hop = new DAL.HopArrival()
            {
                Code = "BNDA01"
            };

            DAL.Parcel DALp = new DAL.Parcel()
            {
                TrackingId = trackingId,
                Receipient = customer,
                Sender = customer
            };

            DALp.FutureHops.Add(hop);
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(DALp);
            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            // Act
            var result = _controller.ReportDeliveryFinal(trackingId);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void StaffReportHop_ShouldReturnTrue_WhenSucceeds_Warehouse()
        {
            string trackingId = "PYJRB4HZ6";
            string code = "BNDA04";
            DAL.Receipient customer = new DAL.Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            var hop = new DAL.HopArrival()
            {
                Code = "BNDA04"
            };

            DAL.Parcel DALp = new DAL.Parcel()
            {
                TrackingId = trackingId,
                Receipient = customer,
                Sender = customer
            };

            DAL.Warehouse wh = new DAL.Warehouse()
            {
                Code = "ABC233",
                Description = "Warehouse",
                HopType = "Warehouse"
            };

            DALp.FutureHops.Add(hop);
            DALp.FutureHops.Add(hop);
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(DALp);
            _mockHOP.Setup(x => x.GetByCode(code)).Returns(wh);
            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            // Act
            var result = _controller.StaffReportHop(trackingId,code);

            // Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void StaffReportHop_ShouldReturnTrue_WhenSucceeds_Truck()
        {
            string trackingId = "PYJRB4HZ6";
            string code = "BNDA04";
            DAL.Receipient customer = new DAL.Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            var hop = new DAL.HopArrival()
            {
                Code = "BNDA04"
            };

            DAL.Parcel DALp = new DAL.Parcel()
            {
                TrackingId = trackingId,
                Receipient = customer,
                Sender = customer
            };

            DAL.Warehouse wh = new DAL.Warehouse()
            {
                Code = "ABC233",
                Description = "Truck",
                HopType = "Truck"
            };

            DALp.FutureHops.Add(hop);
            DALp.FutureHops.Add(hop);
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(DALp);
            _mockHOP.Setup(x => x.GetByCode(code)).Returns(wh);
            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            //moq
            // Act
            var result = _controller.StaffReportHop(trackingId, code);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void StaffReportHop_ShouldReturnFalse_WhenNOTSucceeds()
        {
            string trackingId = "FALSCH";
            string code = "BNDA04";
            DAL.Parcel p = null;
            DAL.Hop h = null;
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Returns(p);
            _mockHOP.Setup(x => x.GetByCode(code)).Returns(h);
            // Act
            var result = _controller.StaffReportHop(trackingId,code);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void StaffReportHop_ShouldReturnDALExceptopn_WhenNotFailsInDAL()
        {
            string trackingId = "FALSCH";
            string code = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new DAL.DALException("Err"));
            BLException exc = Assert.Throws<BLException>(() => _controller.StaffReportHop(trackingId, code));
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
        [Test]
        public void StaffReportHop_ShouldReturnBLExceptopn_WhenNotFailsInBL()
        {
            string trackingId = "FALSCH";
            string code = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new Exception("Err"));
            BLException exc = Assert.Throws<BLException>(() => _controller.StaffReportHop(trackingId, code));
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
    }
}
