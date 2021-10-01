using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Configuration
{
    public class RentalConfiguration : IEntityTypeConfiguration<RentalEntity>
    {
        public void Configure(EntityTypeBuilder<RentalEntity> builder)
        {
            this.MapProperties(builder);
        }

        protected virtual void MapProperties(EntityTypeBuilder<RentalEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
