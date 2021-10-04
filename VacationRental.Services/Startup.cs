using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using VacationRental.Dal.InMemory;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Dal.PostgreSql;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;
using VacationRental.Services.Validation;

namespace VacationRental.Services
{
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        private static Action<IMapperConfigurationExpression> autoMapperConfig =
            config =>
            {
                config.CreateMap<RentalEntity, RentalViewModel>().ReverseMap();
                config.CreateMap<RentalEntity, ResourceIdViewModel>();
                config.CreateMap<RentalBindingModel, RentalEntityCreate>();
                config.CreateMap<PutRentalRequest, RentalEntity>()
                    .ForMember(d => d.Id, opt => opt.MapFrom(x => x.RentalId));
                config.CreateMap<RentalBindingModel, RentalEntity>();
                config.CreateMap<BookingEntity, BookingViewModel>()
                      .ForMember(d => d.Unit, opt => opt.MapFrom(x => x.UnitId))
                      .ForMember(d => d.Nights, opt => opt.MapFrom(x => x.BookingNights))
                      .ForMember(d => d.Start, opt => opt.MapFrom(x => x.BookingStart))
                      .ReverseMap();
                config.CreateMap<BookingEntity, ResourceIdViewModel>();
                config.CreateMap<BookingBindingModel, BookingEntityCreate>();
            };

        public static IServiceCollection ConfigureServiceDependencies(this IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(autoMapperConfig, typeof(Profile).Assembly);

            services.AddScoped<IRentalsService, RentalsService>();
            services.AddScoped<IBookingsService, BookingsService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IBookingValidationService, BookingValidationService>();
            services.AddScoped<IRentalValidationService, RentalValidationService>();
            services.AddScoped<ICalendarValidationService, CalendarValidationService>();
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Use InMemory database
                services.ConfigureInMemoryDataDependencies();
            }
            else
            {
                // Use PostgreSQL database in docker
                services.ConfigurePostgreSqlDataDependencies(connectionString);
            }

            return services;
        }
    }
}
