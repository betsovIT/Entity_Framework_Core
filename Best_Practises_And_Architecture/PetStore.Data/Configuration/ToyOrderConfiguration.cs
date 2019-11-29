using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Data.Models;

namespace PetStore.Data.Configuration
{
    public class ToyOrderConfiguration : IEntityTypeConfiguration<ToyOrder>
    {
        public void Configure(EntityTypeBuilder<ToyOrder> builder)
        {
            builder
                .HasKey(to => new { to.ToyId, to.OrderId });

            builder
                .HasOne(to => to.Toy)
                .WithMany(t => t.Orders)
                .HasForeignKey(to => to.ToyId);

            builder
                .HasOne(to => to.Order)
                .WithMany(to => to.Toys)
                .HasForeignKey(to => to.OrderId);
        }
    }
}
