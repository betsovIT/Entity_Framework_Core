using CarDealer.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext(DbContextOptions<CarDealerContext> options)
            : base(options)
        {
        }

        public CarDealerContext()
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartCar> PartCars { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=CarDealer;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>(entity =>
            {
                entity.HasKey(k => new { k.CarId, k.PartId });

                entity.HasOne(pc => pc.Car).WithMany(c => c.PartCars).HasForeignKey(pc => pc.CarId);
                entity.HasOne(pc => pc.Part).WithMany(c => c.PartCars).HasForeignKey(pc => pc.PartId);
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Supplier).WithMany(s => s.Parts).HasForeignKey(p => p.SupplierId);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
                entity.HasOne(s => s.Car).WithMany(c => c.Sales).HasForeignKey(s => s.CarId);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
        }
    }
}
