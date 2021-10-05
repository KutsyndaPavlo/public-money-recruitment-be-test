using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
    {
        public void Configure(EntityTypeBuilder<BookingEntity> builder)
        {
            this.MapProperties(builder);
        }

        protected virtual void MapProperties(EntityTypeBuilder<BookingEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
