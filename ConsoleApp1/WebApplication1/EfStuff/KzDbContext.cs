﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.EfStuff.Model;
using WebApplication1.EfStuff.Model.Airport;

namespace WebApplication1.EfStuff
{
    public class KzDbContext : DbContext
    {
        public DbSet<Citizen> Citizens { get; set; }

        public DbSet<Adress> Adress { get; set; }
        public DbSet<IncomingFlightInfo> IncomingFlightsInfo { get; set; }
        public DbSet<DepartingFlightInfo> DepartingFlightsInfo { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public KzDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizen>()
                .HasOne(x => x.House)
                .WithMany(x => x.Citizens);

            base.OnModelCreating(modelBuilder);
        }
    }
}
