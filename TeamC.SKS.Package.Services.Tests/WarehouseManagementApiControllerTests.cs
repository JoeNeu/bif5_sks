using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TeamC.SKS.Package.Services.Controllers;
using TeamC.SKS.Package.Services.DTOs.Models;
using TeamC.SKS.Package.Services.Mapper;
using TeamC.SKS.BusinessLogic.Interfaces;
using BL = TeamC.SKS.BusinessLogic.Entities;
using TeamC.SKS.BusinessLogic;
using Moq;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.Package.Services.Tests
{
    [TestFixture]
    public class WarehouseManagementApiControllerTests
    {
        IMapper _mapper;
        Mock<IWarehouseManagementLogic> _mock;
        private WarehouseManagementApi _controller;
        public WarehouseManagementApiControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Mapper.SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IWarehouseManagementLogic>();
            _controller = new WarehouseManagementApi(_mock.Object, _mapper, new LoggerFactory().CreateLogger<WarehouseManagementApi>());
        }

        public void Dispose()
        {
            _mock.Reset();
        }

        [Test]
        public void ExportWarehouses_ValidWarehousesLoaded_200()
        {
            // Arrange
            BL.Warehouse root = new BL.Warehouse() { Code = "TEST"};

            _mock.Setup(x => x.ExportHierarchy()).Returns(root);

            // Act
            var result = (ObjectResult)_controller.ExportWarehouses();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void ExportWarehouses_No_Warehouse_Exists_404Error()
        {
            // Arrange
            BL.Warehouse root = new BL.Warehouse();

            _mock.Setup(x => x.ExportHierarchy()).Returns(root);

            // Act
            var result = (StatusCodeResult)_controller.ExportWarehouses();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ExportWarehouses_WarehousesLoaded_Fail_400Error()
        {
            // Arrange
            BL.Warehouse root = new BL.Warehouse();

            _mock.Setup(x => x.ExportHierarchy()).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ExportWarehouses();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ExportWarehouses_WarehousesLoaded_Fail_400Error_WhenFailInBL()
        {
            // Arrange
            BL.Warehouse root = new BL.Warehouse();

            _mock.Setup(x => x.ExportHierarchy()).Throws(new BL.BLException("Err"));

            // Act
            var result = (ObjectResult)_controller.ExportWarehouses();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void ImportWarehouses_Successfully_Loaded_200()
        {
            // Arrange
            Warehouse DTOroot = new Warehouse();

            _mock.Setup(x => x.ImportHierarchy(It.IsAny<BL.Warehouse>()));

            // Act
            var result = (StatusCodeResult)_controller.ImportWarehouses(DTOroot);
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ImportWarehouses_Unsuccessfully_loaded_400Error()
        {
            Warehouse DTOroot = new Warehouse();

            _mock.Setup(x => x.ImportHierarchy(It.IsAny<BL.Warehouse>())).Throws(new Exception());

            // Act
            var result = (ObjectResult)_controller.ImportWarehouses(DTOroot);
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }
        [Test]
        public void ImportWarehouses_Fail_400Error_WhenFailInBL()
        {
            // Arrange
            Warehouse DTOroot = new Warehouse();
            _mock.Setup(x => x.ImportHierarchy(It.IsAny<BL.Warehouse>())).Throws(new BL.BLException("Err"));

            // Act
            var result = (ObjectResult)_controller.ImportWarehouses(DTOroot);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public void ImportWarehouses_Fail_400Error_WhenFailInService()
        {
            // Arrange
            Warehouse DTOroot = new Warehouse();
            _mock.Setup(x => x.ImportHierarchy(It.IsAny<BL.Warehouse>())).Throws(new Exception("Err"));

            // Act
            var result = (ObjectResult)_controller.ImportWarehouses(DTOroot);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}