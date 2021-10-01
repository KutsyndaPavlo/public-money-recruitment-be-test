using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Dal.InMemory.Repositories;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.InMemory
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInMemoryDataDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRentalsRepository, RentalsRepository>();
            services.AddScoped<IBookingsRepository, BookingsRepository>();

            services.AddDbContext<VacationRentalDbContext>(options => options.UseInMemoryDatabase(databaseName: "VacationRentalDb"));

            return services;
        }
    }
}
