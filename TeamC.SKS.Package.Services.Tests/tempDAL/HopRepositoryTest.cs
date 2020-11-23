using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.DataAccess.Sql;
using System.Linq;
using TeamC.SKS.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Geocoding;

namespace TeamC.SKS.DataAccess.Tests
{
    public class HopRepositoryTest
    {
        private IHopRepository _dal;
        
        [SetUp]
        public void Setup()
        {
            DbContextOptions<SqlContext> options = new DbContextOptionsBuilder<SqlContext>()
               .UseInMemoryDatabase(databaseName: TestContext.CurrentContext.Test.Name)
               .Options;
            _dal = new SqlHopRepository(new SqlContext(options), new LoggerFactory().CreateLogger<SqlHopRepository>());
        }

        [Test]
        public void CreateHop_Succeeded()
        {
            Hop hop = new Hop()
            {
                Code = "ABCDEF123"
            };
            string code = _dal.Create(hop);

            Assert.AreEqual("ABCDEF123", code);
        }

        [Test]
        public void CreateHop_SqlException()
        {
            Hop hop = new Hop()
            {
                Code = "ABCDEF123"
            };
            string code = _dal.Create(hop);
            DALException ex = Assert.Throws<DALException>(() => _dal.Create(hop));
            StringAssert.StartsWith("An item with the same key has already been added", ex.InnerException.Message);
        }

        [Test]
        public void GetHop_ByCode_Succeeded()
        {
            string given = "ABC";
            Hop hop = new Hop() { Code = given };
            _dal.Create(hop);

            hop = _dal.GetByCode(given);

            Assert.NotNull(hop);
            Assert.AreEqual(hop.Code, given);
        }

        [Test]
        public void GetHop_ByCode_Failed()
        {
            string given = "DEF";
            Hop hop = _dal.GetByCode(given);

            Assert.Null(hop);
        }

        [Test]
        public void GetHop_ByHopType_Succeeded()
        {
            string given = "Truck";
            Hop hop = new Hop() { 
                Code = "AUTO123",
                HopType = given };
            _dal.Create(hop);

            hop = _dal.GetByHopType(given);

            Assert.NotNull(hop);
            Assert.AreEqual(hop.HopType, given);
        }

        [Test]
        public void GetHop_ByHopType_Failed()
        {
            string given = "Any";
            Hop hop = _dal.GetByHopType(given);

            Assert.Null(hop);
        }

        [Test]
        public void UpdateHop_Succeeded()
        {
            Hop hop = new Hop() { 
                Code = "ABCDEF123",
                Description = "This WH is awesome" 
            };
            string code = _dal.Create(hop);
            Assert.AreEqual("ABCDEF123", code);

            hop.Description = "This is even better";

            _dal.Update(hop);

            hop = _dal.GetByCode(code);
            Assert.AreEqual("This is even better", hop.Description);
        }

        [Test]
        public void DeleteHop_Succeeded()
        {
            Hop hop = new Hop()
            {
                Code = "ABCDEF123",
                Description = "This WH is awesome"
            };
            string code = _dal.Create(hop);
            Assert.AreEqual("ABCDEF123", code);

            _dal.Delete(code);

            hop = _dal.GetByCode(code);
            Assert.Null(hop);
        }

        [Test]
        public void DeleteHop_Failed()
        {
            string given = "ABCDEF123";
            DALException ex = Assert.Throws<DALException>(() => _dal.Delete(given));
            Assert.AreEqual("System.InvalidOperationException Exception in Delete", ex.Message);
        }


        [Test]
        public void GetWarehouseRoot_success()
        {
            Warehouse hop = new Warehouse()
            {
                Code = "AUTA05",
                Description = "This WH is awesome",
                HopType = "Warehouse",
                NextHops = new List<WarehouseNextHops>()
                {
                    new WarehouseNextHops()
                    {
                        HopACode = "AUTA05",
                        HopBCode = "BNDA04",
                        HopB = new Warehouse()
                        {
                            Code = "BNDA04"
                        }
                    }
                }
            };
            string code = _dal.Create(hop);
            var hop2 = _dal.GetWarehouseRoot();
            Assert.NotNull(hop2);
        }

        [Test]
        public void GetParent_success()
        {
            Warehouse hop = new Warehouse()
            {
                Code = "AUTA05",
                Description = "This WH is awesome",
                HopType = "Warehouse",
                NextHops = new List<WarehouseNextHops>()
                {
                    new WarehouseNextHops()
                    {
                        HopACode = "AUTA05",
                        HopBCode = "BNDA04",
                        HopB = new Warehouse()
                        {
                            Code = "BNDA04"
                        }
                    }
                }
            };
            Warehouse HopB = new Warehouse()
            {
                Code = "BNDA04"
            };
            string code = _dal.Create(hop);
            Hop parent = _dal.GetParent(HopB);
            Assert.IsNotNull(parent);
        }

        [Test]
        public void ClearDatabase()
        {
            Warehouse hop = new Warehouse()
            {
                Code = "BNDA04"
            };
            string code = _dal.Create(hop);
            Assert.NotNull(code);
            _dal.ClearDatabase();
            var hopback = _dal.GetByCode(code);
            Assert.IsNull(hopback);
        }
    }
}
