using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){

        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<AppSession> AppSessions { get; set; } = null!;
        public DbSet<DailySummary> DailySummaries { get; set; } = null!;
        public DbSet<Prediction> Predictions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u=>u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.ApiKey)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => new { d.UserId, d.DeviceName })
                .IsUnique();

            modelBuilder.Entity<AppSession>()
                .HasIndex(a => new {a.DeviceId, a.StartTime})
                .IsUnique();

            modelBuilder.Entity<Prediction>()
                .HasIndex(p => new { p.UserId, p.PredictionForDate })
                .IsUnique();
        }
    }
}