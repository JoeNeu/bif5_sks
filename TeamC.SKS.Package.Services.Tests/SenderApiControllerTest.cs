using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Faker;
using NUnit.Compatibility;
using NUnit.Framework;
using TeamC.SKS.BusinessLogic.Interfaces;
using BL = TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.Package.Services.Controllers;
using TeamC.SKS.Package.Services.DTOs.Models;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    public class SenderApiControllerTest
    {
        IMapper _mapper;
        Mock<ISenderLogic> _mock;
        Parcel parcel;
        private SenderApi _controller;
        public SenderApiControllerTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<ISenderLogic>();
            _controller = new SenderApi(_mock.Object, _mapper, new LoggerFactory().CreateLogger<SenderApi>());

            parcel = new Parcel();
        }

        public void Dispose()
        {
            _mock.Reset();
        }

        [Test]
        public void SubmitParcel_Should_Return_StatusCode_200_When_successfully_submitted_parcel()
        {
            // Arrange
            //var newParcelInfo = Builder<NewParcelInfo>.CreateNew().Build();
            BL.Parcel parcelBL = new BL.Parcel();
            var trackingId = "PYJRB4HZ6";
            NewParcelInfo p = new NewParcelInfo() { TrackingId = trackingId };
            _mock.Setup(x => x.SubmitParcelIntoBL(
                It.IsAny<BL.Parcel>()))
                .Returns(p);

            // Act
            ObjectResult result = (ObjectResult)_controller.SubmitParcel(parcel);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void SubmitParcel_Should_Return_StatusCode_400_When_unsuccessfully_submitted_parcel()
        {
            // Arrange
            BL.Parcel parcelBL = new BL.Parcel();
            var trackingId = "PYJRB4HZ6";
            NewParcelInfo p = new NewParcelInfo() { TrackingId = trackingId };
            _mock.Setup(x => x.SubmitParcelIntoBL(
                It.IsAny<BL.Parcel>()))
                .Throws(new Exception());

            // Act
            ObjectResult result = (ObjectResult)_controller.SubmitParcel(parcel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
