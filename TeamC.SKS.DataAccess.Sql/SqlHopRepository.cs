using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.DataAccess.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace TeamC.SKS.DataAccess.Sql
{
    public class SqlHopRepository : IHopRepository
    {
        private readonly SqlContext _dbContext;
        private readonly ILogger _logger;

        public SqlHopRepository(SqlContext dbContext, ILogger<SqlHopRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogTrace("created SqlHopRepository");
        }
        public string Create(Hop hop)
        {
            try
            {
                _dbContext.Hops.Add(hop);
                _dbContext.SaveChanges();
                return hop.Code;
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

        public void Delete(string code)
        {
            try
            {
                _dbContext.Remove(_dbContext.Hops.Single(x => x.Code == code));
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

        public Hop GetByHopType(string hopType)
        {
            try
            {
                return _dbContext.Hops.FirstOrDefault(x => x.HopType == hopType);
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

        public Hop GetByCode(string code)
        {
            try
            {
                return _dbContext.Hops.FirstOrDefault(x => x.Code == code);
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

        public void Update(Hop h)
        {
            try
            {
                Hop hop = GetByCode(h.Code);
                if (hop != null)
                {
                    _dbContext.Entry(hop).CurrentValues.SetValues(h);
                    _dbContext.SaveChanges();
                }
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

        public Warehouse GetWarehouseRoot()
        {
            try
            {
                Hop child = _dbContext.Warehouses.FirstOrDefault();
                Hop parent = null;
                do
                {
                    parent = GetParent(child);
                    if (parent == null) break;
                    child = parent;
                } while (parent != null);
                var root = child;
                return _dbContext.Warehouses
                        .Include(x => x.NextHops)
                            .ThenInclude(x => x.HopB)
                        .AsEnumerable() //crazy shit
                        .Where(x => x.Code == root.Code)
                        .ToList()
                        .FirstOrDefault();
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

        public void ClearDatabase()
        {
            try
            {
                _dbContext.Database.EnsureDeleted();
                //_dbContext.Database.ExecuteSqlRaw("EXEC sp_MSForEachTable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
                //_dbContext.Database.ExecuteSqlRaw("EXEC sp_MSForEachTable \"DELETE FROM ?\"");
                //_dbContext.Database.ExecuteSqlRaw("EXEC sp_MSForEachTable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
                _dbContext.Database.EnsureCreated();
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

        public Truck GetTruckByLocation(Geocoding.Location location)
        {
            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var point = geometryFactory.CreatePoint(new Coordinate(location.Longitude, location.Latitude));
                return _dbContext.Hops.OfType<Truck>().SingleOrDefault(p => p.RegionGeometry.Contains(point));
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

        public Hop GetParent(Hop child)
        {
            try
            {
                var parentconnection = _dbContext.WarehouseNextHops.Where(x => x.HopBCode == child.Code).FirstOrDefault();
                if (parentconnection == null) return null;
                var parent = _dbContext.Hops.Where(x => x.Code == parentconnection.HopACode).FirstOrDefault();
                return parent;
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
    }
}
