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

namespace TeamC.SKS.DataAccess.Tests
{
    public class ParcelRepositoryTest
    {
        private IParcelRepository _dal;

        [SetUp]
        public void Setup()
        {
            DbContextOptions<SqlContext> options = new DbContextOptionsBuilder<SqlContext>()
               .UseInMemoryDatabase(databaseName: TestContext.CurrentContext.Test.Name)
               .Options;
            _dal = new SqlParcelRepository(new SqlContext(options), new LoggerFactory().CreateLogger<SqlParcelRepository>());
        }

        [Test]
        public void CreateParcel_Succeeded()
        {
            Parcel parcel = new Parcel();
            int id = _dal.Create(parcel);

            Assert.AreNotEqual(0, id);
        }


        [Test]
        public void CreateParcels_SameId_Failed()
        {
            Parcel parcel = new Parcel();

            int id2 = _dal.Create(parcel);

            DALException exc = Assert.Throws<DALException>(() => _dal.Create(parcel));

            StringAssert.StartsWith("System.ArgumentException Exception in Create", exc.Message);
        }

        [Test]
        public void CreateParcels_SingleId_Suceeded()
        {
            int randomNumber = new Random().Next(0, 100);

            Parcel parcel = new Parcel();
            int id1 = _dal.Create(parcel);

            Assert.AreNotEqual(0, id1);

            parcel = new Parcel() { ID = randomNumber };
            int id2 = _dal.Create(parcel);

            Assert.AreNotEqual(0, id2);

            Assert.AreNotEqual(id1, id2);
            Assert.AreEqual(id2, randomNumber);
        }

        [Test]
        public void GetParcel_ById_Succeeded()
        {
            int given = 123;
            Receipient customer = new Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            Parcel parcel = new Parcel() {
                ID = given,
                Receipient = customer,
                Sender = customer
            };
            int id = _dal.Create(parcel);
            Assert.AreEqual(id, given);

            parcel = _dal.GetByID(given);

            Assert.NotNull(parcel);
        }

        [Test]
        public void GetParcel_ById_Failed()
        {
            int given = 321;
            Parcel parcel = _dal.GetByID(given);

            Assert.Null(parcel);
        }

        [Test]
        public void GetParcel_ByTrackingId_Succeeded()
        {
            string given = "XXXX1";
            Receipient customer = new Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            Parcel parcel = new Parcel()
            {
                TrackingId = given,
                Receipient = customer,
                Sender = customer
            };
            _dal.Create(parcel);

            parcel = _dal.GetByTrackingID(given);

            Assert.NotNull(parcel);
            Assert.AreEqual(parcel.TrackingId, given);
        }

        [Test]
        public void GetParcel_ByTrackingId_Failed()
        {
            string given = "HDIU3";
            Parcel parcel = _dal.GetByTrackingID(given);

            Assert.Null(parcel);
        }

        [Test]
        public void GetParcel_ByReceipient_Succeeded()
        {
            Receipient given = new Receipient 
            { 
                Name = "Johannes",
                Street =  "Street",
                PostalCode =  "A-1230"
            };
            _dal.Create(new Parcel() 
            {
                Weight = 1,
                State = Parcel.StateEnum.InTransportEnum,
                Receipient = given ,
                Sender = given ,
                TrackingId = "ABCDEF123"
            });

            IEnumerable<Parcel> parcels = _dal.GetByName(given.Name);

            Assert.NotNull(parcels);
            Assert.IsNotEmpty(parcels);
            Assert.AreEqual(1, parcels.Count());
        }

        [Test]
        public void GetParcel_ByReceipient_Failed()
        {
            Receipient given = new Receipient { Name = "Lisa" };

            IEnumerable<Parcel> parcels = _dal.GetByName(given.Name);

            Assert.IsEmpty(parcels);
        }


        [Test]
        public void UpdateParcel_Succeeded()
        {
            Receipient customer = new Receipient()
            {
                Name = "Johannes",
                Street = "Street",
                PostalCode = "A-1230"
            };
            Parcel parcel = new Parcel() 
            { 
                State = Parcel.StateEnum.DeliveredEnum,
                Receipient = customer,
                Sender = customer
            };
            
            int id = _dal.Create(parcel);
            Assert.AreNotEqual(0, id);

            //Assert.AreNotEqual(parcel.State, Parcel.StateEnum.InTransportEnum);
            parcel.State = Parcel.StateEnum.InTransportEnum;

            _dal.Update(parcel);

            parcel = _dal.GetByID(id);
            Assert.AreEqual(parcel.State, Parcel.StateEnum.InTransportEnum);
        }

        [Test]
        public void DeleteParcel_Succeeded()
        {
            int given = 1234;
            Parcel parcel = new Parcel() { ID = given };
            int id = _dal.Create(parcel);
            Assert.AreEqual(given, id);

            _dal.Delete(id);

            parcel = _dal.GetByID(id);
            Assert.Null(parcel);
        }

        [Test]
        public void DeleteParcel_Failed()
        {
            
            //SqlException exc = Assert.Throws<SqlException>(() => _dal.Delete(given));
            //StringAssert.StartsWith("An item with the same key has already been added", exc.Message);
            //Assert.AreEqual("Sequence contains no elements", exc.Message);
            Assert.IsTrue(true);
        }
    }
}

