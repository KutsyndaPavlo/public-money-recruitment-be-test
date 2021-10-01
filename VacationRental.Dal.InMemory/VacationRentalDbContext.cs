using Microsoft.EntityFrameworkCore;
using VacationRental.Dal.InMemory.Configuration;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory
{
    public class VacationRentalDbContext : DbContext
    {
        public VacationRentalDbContext(DbContextOptions<VacationRentalDbContext> options)
            : base(options) { }

        public virtual DbSet<BookingEntity> Bookings { get; set; }

        public virtual DbSet<RentalEntity> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RentalConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
