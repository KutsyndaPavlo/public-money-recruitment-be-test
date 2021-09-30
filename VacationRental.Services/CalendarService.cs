using AutoMapper;
using System;
using System.Collections.Generic;
using VacationRental.Data.Repositiries;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IRentalsRepository _rentalsRepository;
        private readonly IMapper _mapper;

        public CalendarService(IBookingsRepository bookingsRepository, IRentalsRepository rentalsRepository, IMapper mapper)
        {
            _bookingsRepository = bookingsRepository;
            _rentalsRepository = rentalsRepository;
            _mapper = mapper;
        }
        public ServiceResponse<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
        {
            var bookings = _bookingsRepository.GetBookings(rentalId, start, start.AddDays(nights));

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>(),
                
            };

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                foreach (var booking in bookings)
                {
                    if (booking.BookingStart <= date.Date && booking.BookingEnd >= date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = booking.UnitId });
                        continue;
                    }

                    if (booking.PreparationStart <= date.Date && booking.PreparationEnd >= date.Date)
                    {
                        date.PreparationTimes.Add(new CalendarPreparationTimeViewModel { Unit = booking.UnitId });
                    }
                }

                result.Dates.Add(date);
            }

            return new ServiceResponse<CalendarViewModel> { Result = result, Status = ResponseStatus.Success };

            //{ Dates:[ { Date: “2031 - 01 - 01”, Bookings:[ { Id: 2, Unit: 1 } ], PreparationTimes:[] }, 
            //          { Date: “2031 - 01 - 02”, Bookings:[ { Id: 2, Unit: 1 } ], PreparationTimes:[] }, 
            //          { Date: “2031 - 01 - 03”, Bookings:[], PreparationTimes:[ { Unit: 1 } ] } ] }
        }
    }
}
