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
using TeamC.SKS.DataAccess.Interfaces;
using BL = TeamC.SKS.BusinessLogic.Entities;
using DAL = TeamC.SKS.DataAccess.Entities;
using TeamC.SKS.Package.Services.Controllers;
using TeamC.SKS.Package.Services.DTOs.Models;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    public class ParcelWebhookApiControllerTest
    {
        IMapper _mapper;
        Mock<IWebhookRepository> _mock;
        Parcel parcel;
        private ParcelWebhookApi _controller;
        public ParcelWebhookApiControllerTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IWebhookRepository>();
            _controller = new ParcelWebhookApi(_mock.Object, _mapper, new LoggerFactory().CreateLogger<ParcelWebhookApi>());

            parcel = new Parcel();
        }

        [Test]
        public void ApiParcelByTrackingIdWebhooksGet_Should_Return_StatusCode_200_When_successfully()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";
            List<DAL.Webhook> hooklist = new List<DAL.Webhook>() 
            {
                new DAL.Webhook()
                {
                    Id = 1,
                    TrackingId = trackingId
                },
                new DAL.Webhook()
                {
                    Id = 2,
                    TrackingId = trackingId
                }
            };
            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Returns(true);
            _mock.Setup(x => x.GetWebhooksByTrackingID(trackingId)).Returns(hooklist);

            // Act
            ObjectResult result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksGet(trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksGet_Should_Return_StatusCode_400_When_unsuccessfully()
        {
            var trackingId = "PYJRB4HZ6";
            // Arrange
            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Returns(false);

            // Act
            StatusCodeResult result = (StatusCodeResult)_controller.ApiParcelByTrackingIdWebhooksGet(trackingId);

            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksPost_Should_Return_StatusCode_200_When_successfully()
        {
            // Arrange
            long id = 1;
            var trackingId = "PYJRB4HZ6";
            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Returns(true);
            _mock.Setup(x => x.Create(It.IsAny<DAL.Webhook>())).Returns(id);

            // Act
            ObjectResult result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksPost(trackingId, trackingId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksPost_Should_Return_StatusCode_400_When_unsuccessfully()
        {
            var trackingId = "PYJRB4HZ6";
            // Arrange
            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Returns(false);

            // Act
            StatusCodeResult result = (StatusCodeResult)_controller.ApiParcelByTrackingIdWebhooksPost(trackingId, trackingId);

            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void ApiParcelWebhooksByIdDelete_Should_Return_StatusCode_200_When_successfully()
        {
            // Arrange
            long id = 1;
            _mock.Setup(x => x.GetWebhookByID(id)).Returns(true);
            _mock.Setup(x => x.Delete(id));

            // Act
            StatusCodeResult result = (StatusCodeResult)_controller.ApiParcelWebhooksByIdDelete(id);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ApiParcelWebhooksByIdDelete_Should_Return_StatusCode_400_When_unsuccessfully()
        {
            long id = 1;
            // Arrange
            _mock.Setup(x => x.GetWebhookByID(id)).Returns(false);

            // Act
            StatusCodeResult result = (StatusCodeResult)_controller.ApiParcelWebhooksByIdDelete(id);

            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksGet_Fail_400Error()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksGet(trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksGet_Fail_400Error_WhenFailInBL()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Throws(new BL.BLException("Err"));

            // Act
            var result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksGet(trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksPost_Fail_400Error()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksPost(trackingId, trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ApiParcelByTrackingIdWebhooksPost_Fail_400Error_WhenFailInBL()
        {
            // Arrange
            var trackingId = "PYJRB4HZ6";

            _mock.Setup(x => x.GetParcelByTrackingID(trackingId)).Throws(new BL.BLException("Err"));

            // Act
            var result = (ObjectResult)_controller.ApiParcelByTrackingIdWebhooksPost(trackingId, trackingId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ApiParcelWebhooksByIdDelete_Fail_400Error()
        {
            // Arrange
            long Id = 1;

            _mock.Setup(x => x.GetWebhookByID(Id)).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ApiParcelWebhooksByIdDelete(Id);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ApiParcelWebhooksByIdDelete_Fail_400Error_WhenFailInBL()
        {
            // Arrange
            long Id = 1;

            _mock.Setup(x => x.GetWebhookByID(Id)).Throws(new BL.BLException("Err"));

            // Act
            var result = (ObjectResult)_controller.ApiParcelWebhooksByIdDelete(Id);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
