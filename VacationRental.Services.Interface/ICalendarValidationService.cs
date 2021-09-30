using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface ICalendarValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetCalendarRequest requestData);
    }
}
