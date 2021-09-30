using System;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface ICalendarService
    {
        ServiceResponse<CalendarViewModel> Get(GetCalendarRequest request);
    }
}