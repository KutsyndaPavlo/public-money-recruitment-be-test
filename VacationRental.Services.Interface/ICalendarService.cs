using System.Threading.Tasks;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface
{
    public interface ICalendarService
    {
        Task<ServiceResponse<CalendarViewModel>> GetAsync(GetCalendarRequest request);
    }
}