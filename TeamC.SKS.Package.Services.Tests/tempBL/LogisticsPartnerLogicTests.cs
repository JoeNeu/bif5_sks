using AutoMapper;
using Moq;
using NUnit.Framework;
using TeamC.SKS.BusinessLogic.Entities;
using DAL = TeamC.SKS.DataAccess.Entities;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.Package.Services.Mapper;
using System;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic.Tests
{
    [TestFixture]
    public class LogisticsPartnerLogicTests
    {
        IMapper _mapper;
        Mock<IParcelRepository> _mock;
        private LogisticsPartnerLogic _controller;

        public LogisticsPartnerLogicTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IParcelRepository>();
            _controller = new LogisticsPartnerLogic(_mock.Object, _mapper, new LoggerFactory().CreateLogger<LogisticsPartnerLogic>());
        }
        [Test]
        public void TransferParcelPartner_ShouldsafeOldTrackingID_WhenSucceeds()
        {
            string newtrackingId = "ABCDE";
            string oldtrackingId = "XXXXX";
            DAL.Parcel p = new DAL.Parcel()
            {
                TrackingId = "ABCDE"
            };
            _mock.Setup(x => x.GetByTrackingID(newtrackingId)).Returns(p);
            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            // Act

            _controller.TransferParcelPartner(newtrackingId, oldtrackingId);
            // Assert
            Assert.AreEqual(oldtrackingId, p.OldTrackingId);
        }
        [Test]
        public void TransferParcelPartner_ShouldReturnNull_WhenNOTSucceeds()
        {
            string newtrackingId = "FALSCH";
            string oldtrackingId = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(newtrackingId)).Throws(new Exception("hi"));

            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            BLException exc = Assert.Throws<BLException>(() => _controller.TransferParcelPartner(newtrackingId,oldtrackingId));

            Assert.AreEqual($"hi", exc.InnerException.Message);
        }
        [Test]
        public void TransferParcelPartner_ShouldReturnNull_WhenNOTSucceedsInDAL()
        {
            string newtrackingId = "FALSCH";
            string oldtrackingId = "FALSCH";
            _mock.Setup(x => x.GetByTrackingID(newtrackingId)).Throws(new DAL.DALException("hi"));

            _mock.Setup(x => x.Update(It.IsAny<DAL.Parcel>()));
            BLException exc = Assert.Throws<BLException>(() => _controller.TransferParcelPartner(newtrackingId, oldtrackingId));

            Assert.AreEqual($"hi", exc.InnerException.Message);
        }
    }
}