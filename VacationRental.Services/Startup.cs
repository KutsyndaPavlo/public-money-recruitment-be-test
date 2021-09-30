﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface.Models;
using VacationRental.Dal.InMemory;

namespace VacationRental.Services
{
    public static class Startup
    {
        private static Action<IMapperConfigurationExpression> autoMapperConfig =
            config =>
            {
                config.CreateMap<RentalEntity, RentalViewModel>().ReverseMap();
                config.CreateMap<RentalEntity, ResourceIdViewModel>();
                config.CreateMap<RentalBindingModel, RentalEntityCreate>();
                config.CreateMap<RentalBindingModel, RentalEntity>();
                config.CreateMap<BookingEntity, BookingViewModel>()
                      .ForMember(d => d.Unit, opt => opt.MapFrom(x => x.UnitId))
                      .ForMember(d => d.Nights, opt => opt.MapFrom(x => x.BookingNights))
                      .ForMember(d => d.Start, opt => opt.MapFrom(x => x.BookingStart))
                      .ReverseMap();
                config.CreateMap<BookingEntity, ResourceIdViewModel>();
                config.CreateMap<BookingBindingModel, BookingEntityCreate>();
            };

        public static IServiceCollection ConfigureServiceDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(autoMapperConfig, typeof(Profile).Assembly);

            services.ConfigureDataDependencies();

            return services;
        }
    }
}
