using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class CalendarValidationService : ICalendarValidationService
    {
        #region Methods

        public ServiceResponse<string>ValidateGetRequest(GetCalendarRequest request)
        {
            return request.Nights <= 0 
                ? new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed,
                    Result = VacationRentalConstants.IncorrectNightsErrorMessage
                } 
                : new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
