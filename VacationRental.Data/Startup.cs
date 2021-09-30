using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VacationRental.Data.Repositiries;

namespace VacationRental.Data
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
