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
    public class WebhookRepositoryTest
    {
        private IWebhookRepository _dal;

        [SetUp]
        public void Setup()
        {
            DbContextOptions<SqlContext> options = new DbContextOptionsBuilder<SqlContext>()
               .UseInMemoryDatabase(databaseName: TestContext.CurrentContext.Test.Name)
               .Options;
            _dal = new SqlWebhookRepository(new SqlContext(options), new LoggerFactory().CreateLogger<SqlWebhookRepository>());
        }

        [Test]
        public void CreateWebhook_Succeeded()
        {
            Webhook webhook = new Webhook();
            long id = _dal.Create(webhook);

            Assert.AreEqual(1, id);
        }
        [Test]
        public void CreateWebhook_SameId_Failed()
        {
            Webhook webhook = new Webhook();
            long id = _dal.Create(webhook);

            DALException exc = Assert.Throws<DALException>(() => _dal.Create(webhook));

            StringAssert.StartsWith("System.ArgumentException Exception in Create", exc.Message);
        }
        [Test]
        public void CreateWebhook_SingleId_Suceeded()
        {
            int randomNumber = new Random().Next(0, 100);

            Webhook webhook = new Webhook();
            long id1 = _dal.Create(webhook);

            Assert.AreNotEqual(0, id1);

            Webhook webhook2 = new Webhook();
            long id2 = _dal.Create(webhook2);

            Assert.AreNotEqual(0, id2);

            Assert.AreNotEqual(id1, id2);
        }
        [Test]
        public void GetWebhook_ById_Succeeded()
        {
            Webhook webhook = new Webhook();
            long id1 = _dal.Create(webhook);


            var gethook = _dal.GetWebhookByID(id1);

            Assert.IsTrue(gethook);
        }
        [Test]
        public void GetWebhook_ById_NoWebhookFound()
        {
            Webhook webhook = new Webhook();
            long id1 = _dal.Create(webhook);
            long id2 = 0;
            Assert.AreNotEqual(id1, id2);

            var gethook = _dal.GetWebhookByID(id2);

            Assert.IsFalse(gethook);
        }
        [Test]
        public void DeleteWebhook_Succeeded()
        {
            Webhook webhook = new Webhook();
            long id1 = _dal.Create(webhook);


            _dal.Delete(id1);

            var parcel = _dal.GetWebhookByID(id1);
            Assert.IsFalse(parcel);
        }
        [Test]
        public void GetParcelByTrackingID()
        {

            string given = "XXXX1";

            var parcel = _dal.GetParcelByTrackingID(given);
            Assert.IsFalse(parcel);
        }
        [Test]
        public void GetWebhooksByTrackingID_succeded()
        {
            string track = "ABCDEF123";
            Webhook webhook = new Webhook() {
                TrackingId = track
            };
            Webhook webhook2 = new Webhook()
            {
                TrackingId = track
            };
            long id1 = _dal.Create(webhook);
            long id2 = _dal.Create(webhook2);

            var list = _dal.GetWebhooksByTrackingID(track);
            Assert.NotNull(list);
        }
    }
}
