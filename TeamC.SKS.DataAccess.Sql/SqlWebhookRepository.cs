using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamC.SKS.DataAccess.Entities;
using TeamC.SKS.DataAccess.Interfaces;

namespace TeamC.SKS.DataAccess.Sql
{
    public class SqlWebhookRepository : IWebhookRepository
    {
        private readonly SqlContext _dbContext;
        private readonly ILogger _logger;

        public SqlWebhookRepository(SqlContext dbContext, ILogger<SqlWebhookRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogTrace("created SqlWebhookRepository");
        }
        public long Create(Webhook hook)
        {
            try
            {
                _dbContext.Webhooks.Add(hook);
                _dbContext.SaveChanges();
                return hook.Id;
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
            catch (Exception exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public void Delete(long id)
        {
            try
            {
                _dbContext.Remove(_dbContext.Webhooks.Single(x => x.Id == id));
                _dbContext.SaveChanges();
            }
            catch (SqlException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
            catch (Exception exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }
        public bool GetParcelByTrackingID(string trackingID)
        {
            try
            {
                if(_dbContext.Parcels.Where(x => x.TrackingId == trackingID).FirstOrDefault() != null) return true;
                return false;
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }
        public bool GetWebhookByID(long id)
        {
            try
            {
                if (_dbContext.Webhooks.Where(x => x.Id == id).FirstOrDefault() != null) return true;
                return false;
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }
        public List<Webhook> GetWebhooksByTrackingID(string trackingID)
        {
            return _dbContext.Webhooks.Where(x => x.TrackingId == trackingID).ToList<Webhook>();
        }
    }
}
