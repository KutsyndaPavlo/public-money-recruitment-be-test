using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ServiceResponse<CalendarViewModel>> GetAsync(GetCalendarRequest request)
        {
            var bookings = await _bookingsRepository.GetBookingsAsync(request.RentalId, request.StartDate, request.StartDate.AddDays(request.Nights));

            var result = new CalendarViewModel
            {
                RentalId = request.RentalId,
                Dates = new List<CalendarDateViewModel>()
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
        }
    }
}
