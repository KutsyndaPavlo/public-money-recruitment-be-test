using Microsoft.Extensions.DependencyInjection;

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
