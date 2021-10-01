using System.Threading.Tasks;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface ICalendarService
    {
        Task<ServiceResponse<CalendarViewModel>> GetAsync(GetCalendarRequest request);
    }
}