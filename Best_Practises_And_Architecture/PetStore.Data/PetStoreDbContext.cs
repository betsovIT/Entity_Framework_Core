using Microsoft.EntityFrameworkCore;
using PetStore.Data.Configuration;
using PetStore.Data.Models;

namespace PetStore.Data
{
    public class PetStoreDbContext : DbContext
    {
        public PetStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public PetStoreDbContext()
        {
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Breed> Breeds { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Food> Food { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Toy> Toys { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<FoodOrder> FoodsOrders { get; set; }

        public DbSet<ToyOrder> ToysOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(DataSetings.Connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
         => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

    }
}
