using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface.Validation
{
    public interface ICalendarValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetCalendarRequest request);
    }
}
