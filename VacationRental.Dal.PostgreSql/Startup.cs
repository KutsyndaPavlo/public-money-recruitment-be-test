using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.PostgreSql
{
    public static class Startup
    {
        public static IServiceCollection ConfigurePostgreSqlDataDependencies(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUnitOfWork>(x=> new UnitOfWork(new NpgsqlConnection(connectionString)));

            return services;
        }
    }
}
