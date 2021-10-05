using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface.Validation
{
    public interface ICalendarValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetCalendarRequest request);
    }
}
