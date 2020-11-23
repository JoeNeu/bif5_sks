using AutoMapper;
using Moq;
using NUnit.Framework;
using TeamC.SKS.BusinessLogic;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.Package.Services.Mapper;
using DAL = TeamC.SKS.DataAccess.Entities;
using BL = TeamC.SKS.BusinessLogic.Entities;
using System;
using Microsoft.Extensions.Logging;

namespace TeamC.SKS.BusinessLogic.Tests
{
    [TestFixture]
    public class WarehouseManagementLogicTest
    {
        IMapper _mapper;
        Mock<IHopRepository> _mock;
        BL.Warehouse wh;
        private WarehouseManagementLogic _controller;

        public WarehouseManagementLogicTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new SKSLayerMapper()));
            _mapper = new AutoMapper.Mapper(config);
            _mock = new Mock<IHopRepository>();
            _controller = new WarehouseManagementLogic(_mock.Object, _mapper, new LoggerFactory().CreateLogger<WarehouseManagementLogic>());


            wh = new BL.Warehouse()
            {
                Description = "BNDA04"
            };
        }
        [Test]
        public void ExportHierarchy_ShouldReturnRoot_WhenSucceeds()
        {
            var DALwh = new DAL.Warehouse();
            _mock.Setup(x => x.GetWarehouseRoot()).Returns(DALwh);
            // Act
            var result = _controller.ExportHierarchy();

            // Assert
            Assert.IsInstanceOf<BL.Warehouse>(result);
            
        }
        [Test]
        public void ExportHierarchy_ShouldReturnDALException_WhenFailsInDAL()
        {
            _mock.Setup(x => x.GetWarehouseRoot()).Throws(new DAL.DALException("Err"));
            BL.BLException exc = Assert.Throws<BL.BLException>(() => _controller.ExportHierarchy());
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
        [Test]
        public void ExportHierarchy_ShouldReturnException_WhenFailsInBL()
        {
            _mock.Setup(x => x.GetWarehouseRoot()).Throws(new Exception("Err"));
            BL.BLException exc = Assert.Throws<BL.BLException>(() => _controller.ExportHierarchy());
            StringAssert.StartsWith("Err", exc.InnerException.Message);
        }
        [Test]
        public void ImportHierarchy_WhenSucceeds()
        {
            _mock.Setup(x => x.Create(It.IsAny<DAL.Warehouse>()));
            Assert.IsTrue(true);
        }
        [Test]
        public void ImportHierarchy_ShouldReturnExection_WhenValidationFails()
        {
            _mock.Setup(x => x.Create(It.IsAny<DAL.Warehouse>()));
            wh.Description = "";
            Exception exc = Assert.Throws<Exception>(() => _controller.ImportHierarchy(wh));

            Assert.AreEqual($"Import Warehouse Tree failed in BL", exc.Message);
        }
        [Test]
        public void ImportHierarchy_ShouldReturnExection_WhenDALSubmitFailsInBL()
        {
            _mock.Setup(x => x.Create(It.IsAny<DAL.Warehouse>()))
                .Throws(new Exception("DAL: submit Root Warehouse failed"));

            BL.BLException exc = Assert.Throws<BL.BLException>(() => _controller.ImportHierarchy(wh));

            StringAssert.Contains("DAL: submit Root Warehouse failed", exc.InnerException.Message);
        }
        [Test]
        public void ImportHierarchy_ShouldReturnExection_WhenDALSubmitFailsInDAL()
        {
            _mock.Setup(x => x.Create(It.IsAny<DAL.Warehouse>()))
                .Throws(new DAL.DALException("DAL: submit Root Warehouse failed"));

            BL.BLException exc = Assert.Throws<BL.BLException>(() => _controller.ImportHierarchy(wh));

            StringAssert.Contains("DAL: submit Root Warehouse failed", exc.InnerException.Message);
        }
    }
}
