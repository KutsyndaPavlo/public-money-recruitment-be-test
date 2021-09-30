using System;
using VacationRental.Api.Models;
using VacationRental.Services.Models;

namespace VacationRental.Services
{
    public interface ICalendarService
    {
        ServiceResponse<CalendarViewModel> Get(int rentalId, DateTime start, int nights);
    }
}