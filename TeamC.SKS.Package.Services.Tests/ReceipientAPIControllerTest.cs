using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.BusinessLogic.Interfaces;
using BL = TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.Package.Services.Controllers;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    class ReceipientAPIControllerTest
    {
        IMapper _mapper;
        Mock<IReceipientLogic> _mock;
        private ReceipientApi _controller;
        public ReceipientAPIControllerTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IReceipientLogic>();
            _controller = new ReceipientApi(_mock.Object, _mapper, new LoggerFactory().CreateLogger<ReceipientApi>());
        }
        [Test]
        public void TrackParcel_Should_Return_StatusCode_200_If_Parcel_Exists()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";
            BL.Parcel p = new BL.Parcel() { TrackingId = trackingId };
            _mock.Setup(x => x.TrackPackage(trackingId)).Returns(p);
            // Act
            var result = (ObjectResult)_controller.TrackParcel(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void TrackParcel_Should_Return_StatusCode_400_If_Error_is_Thrown()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.TrackPackage(trackingId)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.TrackParcel(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public void TrackParcel_Should_Return_StatusCode_404_If_Parcel_Not_Exists()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.TrackPackage(trackingId)).Returns(It.IsAny<BL.Parcel>);

            // Act
            var result = (StatusCodeResult)_controller.TrackParcel(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
