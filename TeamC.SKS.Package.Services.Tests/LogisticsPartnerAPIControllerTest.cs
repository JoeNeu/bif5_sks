using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Compatibility;
using NUnit.Framework;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.Package.Services.Controllers;
using TeamC.SKS.Package.Services.DTOs.Models;
using BL = TeamC.SKS.BusinessLogic.Entities;

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    public class LogisticsPartnerAPIControllerTest
    {
        IMapper _mapper;
        Mock<ILogisticsPartnerLogic> _mockPartner;
        Mock<ISenderLogic> _mockSender;
        private LogisticsPartnerApi _controller;
        public LogisticsPartnerAPIControllerTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mockPartner = new Mock<ILogisticsPartnerLogic>();
            _mockSender = new Mock<ISenderLogic>();
           _controller = new LogisticsPartnerApi(_mockPartner.Object, _mockSender.Object, _mapper, new LoggerFactory().CreateLogger<LogisticsPartnerApi>());

        }
        [Test]
        public void TransitionParcel_Should_Return_StatusCode_200_When_Succeeds()
        {
            // Arrange
            //var controller = new LogisticsPartnerApi(_mockPartner.Object, _mockSender.Object, _mapper, new LoggerFactory().CreateLogger<LogisticsPartnerApi>());
            var parcel = new Parcel();
            var trackingId = "PYJRB4HZ6";
            NewParcelInfo p = new NewParcelInfo() { TrackingId = trackingId };
            _mockSender.Setup(x => x.SubmitParcelIntoBL(It.IsAny<BL.Parcel>())).Returns(p);
            _mockPartner.Setup(x => x.TransferParcelPartner(It.IsAny<string>(), It.IsAny<string>()));
                
            // Act
            var result = (ObjectResult)_controller.TransitionParcel(parcel, trackingId);

            // Assert
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void TransitionParcel_Should_Return_StatusCode_400_When_Failed()
        {
            // Arrange
            var parcel = new Parcel();
            var trackingId = "JHGBVCFR";
            _mockSender.Setup(x => x.SubmitParcelIntoBL(It.IsAny<BL.Parcel>())).Throws(new BL.BLException("failed"));
            _mockPartner.Setup(x => x.TransferParcelPartner(It.IsAny<string>(), It.IsAny<string>()));
            // Act
            var result = (ObjectResult)_controller.TransitionParcel(parcel, trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
