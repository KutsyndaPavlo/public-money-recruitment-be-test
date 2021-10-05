using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class CalendarValidationService : ServiceBase, ICalendarValidationService
    {
        #region Methods

        public ServiceResponse<string>ValidateGetRequest(GetCalendarRequest request)
        {
            return request.Nights <= 0 
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectNightsErrorMessage)
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        #endregion
    }
}
