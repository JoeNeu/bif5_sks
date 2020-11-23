using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamC.SKS.DataAccess.Entities;
using TeamC.SKS.DataAccess.Interfaces;

namespace TeamC.SKS.DataAccess.Sql
{
    public class SqlParcelRepository : IParcelRepository
    {
        private readonly SqlContext _dbContext;
        private readonly ILogger _logger;

        public SqlParcelRepository(SqlContext dbContext, ILogger<SqlParcelRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogTrace("created SqlParcelRepository");
        }

        public int Create(Parcel p)
        {
            try
            {
                _dbContext.Parcels.Add(p);
                _dbContext.SaveChanges();
                return p.ID;
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _dbContext.Remove(_dbContext.Parcels.Single(x => x.ID == id));
                _dbContext.SaveChanges();
            }
            catch (SqlException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public Parcel GetByID(int id)
        {
            try
            {
                return _dbContext.Parcels.Where(x => x.ID == id)
                                            .Include(x => x.Sender)
                                            .Include(x => x.Receipient)
                                            .FirstOrDefault();
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public IEnumerable<Parcel> GetByName(string name)
        {
            try
            {
                return _dbContext.Parcels.Where(x => x.Sender.Name == name || x.Receipient.Name == name).AsEnumerable();
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public Parcel GetByTrackingID(string trackingID)
        {
            try
            {
                return _dbContext.Parcels.Where(x => x.TrackingId == trackingID)
                                            .Include(Sender => Sender.Sender)
                                            .Include(Receipient => Receipient.Receipient)
                                            .Include(vHop => vHop.VisitedHops)
                                            .Include(fHop => fHop.FutureHops)
                                            .FirstOrDefault();
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }

        public void Update(Parcel p)
        {
            try
            {
                Parcel parcel = GetByID(p.ID);
                if (parcel != null)
                {
                    _dbContext.Entry(parcel).CurrentValues.SetValues(p);
                    _dbContext.SaveChanges();
                }
            }
            catch (ArgumentException exc)
            {
                throw new DALException($"{exc.GetType()} Exception in {System.Reflection.MethodBase.GetCurrentMethod().Name}", exc);
            }
        }
    }
}
