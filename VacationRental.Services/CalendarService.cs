using System;
using System.Collections.Generic;
using VacationRental.Dal.Interface;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IBookingsRepository _bookingsRepository;

        public CalendarService(IBookingsRepository bookingsRepository)
        {
            _bookingsRepository = bookingsRepository;
        }
        public ServiceResponse<CalendarViewModel> Get(GetCalendarRequest request)
        {
            var bookings = _bookingsRepository.GetBookings(request.RentalId, request.StartDate, request.StartDate.AddDays(request.Nights));

            var result = new CalendarViewModel
            {
                RentalId = request.RentalId,
                Dates = new List<CalendarDateViewModel>(),
                
            };

            for (var i = 0; i < request.Nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = request.StartDate.Date.AddDays(i),
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
