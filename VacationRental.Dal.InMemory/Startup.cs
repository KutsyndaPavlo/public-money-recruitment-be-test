using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.InMemory
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInMemoryDataDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>(); 

            services.AddDbContext<VacationRentalDbContext>(options => options
                .UseInMemoryDatabase(databaseName: "VacationRentalDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            return services;
        }
    }
}
