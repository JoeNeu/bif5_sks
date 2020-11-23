using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.DataAccess.Sql
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
        }
        
        protected override void OnModelCreating (ModelBuilder builder)
        {
            builder.Entity<Hop>().HasDiscriminator(p => p.HopType);
            builder.Entity<Warehouse>().HasBaseType<Hop>();
            builder.Entity<Truck>().HasBaseType<Hop>();
            builder.Entity<Transferwarehouse>().HasBaseType<Hop>();

            builder.Entity<WarehouseNextHops>()
                .HasKey(p => new { p.HopACode, p.HopBCode });

            builder.Entity<Warehouse>()
                .HasMany<WarehouseNextHops>(p => p.NextHops)    //1:n Navigation Property
                .WithOne(p => p.HopA)                           //Reverse Navigation Property
                .HasForeignKey(p => p.HopACode)                 //Foreign key of reverse navigation property
                .OnDelete(DeleteBehavior.NoAction);             //prevent circular dependecies

            builder.Entity<Hop>()
                .HasOne(p => p.InboundTransport)                //1:1 Navigation Property
                .WithOne(p => p.HopB);                          //Reverse 1:1 Navigation Property
        }

        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Receipient> Receipients { get; set; }
        public DbSet<HopArrival> HopArrivals { get; set; }
        public DbSet<Hop> Hops { get; set; }
        public DbSet<Transferwarehouse> Transferwarehouses { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<WarehouseNextHops> WarehouseNextHops { get; set; }
        public DbSet<Webhook> Webhooks { get; set; }
    }
}
