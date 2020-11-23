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

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    public class StaffApiControllerTest
    {
        IMapper _mapper;
        Mock<IStaffLogic> _mock;
        private StaffApi _controller;
        public StaffApiControllerTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IStaffLogic>();
            _controller = new StaffApi(_mock.Object, _mapper, new LoggerFactory().CreateLogger<StaffApi>());
        }

        [Test]
        public void ReportParcelDelivery_Should_Return_StatusCode_200_When_Succeeds()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.ReportDeliveryFinal(trackingId)).Returns(true);

            // Act
            var result = (StatusCodeResult)_controller.ReportParcelDelivery(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void ReportParcelDelivery_Should_Return_StatusCode_400_When_Failed()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.ReportDeliveryFinal(trackingId)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ReportParcelDelivery(trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void ReportParcelDelivery_Should_Return_StatusCode_404_When_not_exists()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.ReportDeliveryFinal(trackingId)).Returns(false);

            // Act
            var result = (StatusCodeResult)_controller.ReportParcelDelivery(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ReportParcelHop_Should_Return_StatusCode_200_When_Successfully_Hopped()
        {
            // Arrange
            //var controller = new WarehouseManagementApiController(null, null);
            //var controller = new StaffApi();
            //var parcel = new Parcel();
            var trackingId = "PYJRB4HZ6";
            var code = "BNDA04";
            
            _mock.Setup(x => x.StaffReportHop(trackingId, code)).Returns(true);

            // Act
            var result = (StatusCodeResult)_controller.ReportParcelHop(trackingId, code);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void ReportParcelHop_Should_Return_StatusCode_404_When_Unsuccessfully_Hipped_Wrong_TrackingID_or_Code()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";
            var code = "BNDA04";

            _mock.Setup(x => x.StaffReportHop(trackingId, code)).Returns(false);
            // Act
            var result = (StatusCodeResult)_controller.ReportParcelHop(trackingId, code);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ReportParcelHop_Should_Return_StatusCode_400_When_Exception_Is_thrown()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";
            var code = "BNDA04";

            //SIMULATE AN ERROR IN BUSINESSLAYER -> throw EXCEPTION
            _mock.Setup(x => x.StaffReportHop(trackingId, code)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ReportParcelHop(trackingId, code);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }
    }
}
