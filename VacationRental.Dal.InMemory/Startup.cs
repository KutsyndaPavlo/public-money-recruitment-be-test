using Microsoft.Extensions.DependencyInjection;
using VacationRental.Dal.InMemory.Repositiries;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.InMemory
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDataDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IRentalsRepository, RentalsRepository>();
            services.AddSingleton<IBookingsRepository, BookingsRepository>();

            return services;
        }
    }
}
