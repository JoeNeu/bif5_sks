using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.Package.Services.Mapper;
using DAL = TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.BusinessLogic.Tests
{
    [TestFixture]
    public class ReceipientLogicTest
    {
        IMapper _mapper;
        Mock<IParcelRepository> _mock;
        private ReceipientLogic _controller;

        public ReceipientLogicTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IParcelRepository>();
            _controller = new ReceipientLogic(_mock.Object, _mapper, new LoggerFactory().CreateLogger<ReceipientLogic>());
        }
        [Test]
        public void TrackPackage_ShouldReturnParcel_WhenValidTrackingID()
        {
            string trackingId = "ABCDEF123";
            DAL.Parcel p = new DAL.Parcel() { TrackingId = "ABCDEF123" };

            _mock.Setup(x => x.GetByTrackingID(trackingId))
                .Returns(p);

            // Act
            var result = _controller.TrackPackage(trackingId);

            // Assert
            Assert.AreEqual(trackingId, result.TrackingId);
        }
        [Test]
        public void TrackPackage_ShouldReturnException_WhenNOTSucceeds()
        {
            string trackingId = "ABCDEF123";
            DAL.Parcel p = new DAL.Parcel() { TrackingId = "ABCDEF123" };

            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new Exception("HINIG"));

            BLException exc = Assert.Throws<BLException>(() => _controller.TrackPackage(trackingId));

            Assert.AreEqual("HINIG", exc.InnerException.Message);
        }
        [Test]
        public void TrackPackage_isNull_WhenNOTFound()
        {
            string trackingId = "ABCDEF123";
            DAL.Parcel p = null;

            _mock.Setup(x => x.GetByTrackingID(trackingId))
                .Returns(p);

            Assert.IsNull(p);
        }
        [Test]
        public void TrackPackage_ShouldReturnException_WhenNOTSucceedsInDAL()
        {
            string trackingId = "ABCDEF123";
            _mock.Setup(x => x.GetByTrackingID(trackingId)).Throws(new DAL.DALException("Err"));
            BLException exc = Assert.Throws<BLException>(() => _controller.TrackPackage(trackingId));
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
    }
}
