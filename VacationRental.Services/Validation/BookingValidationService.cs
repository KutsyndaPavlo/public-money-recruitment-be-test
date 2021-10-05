using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class BookingValidationService : ServiceBase, IBookingValidationService
    {
        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetBookingRequest request)
        {
            return request.BookingId <= 0
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectBookingIdErrorMessage)
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        public ServiceResponse<string> ValidatePostRequest(BookingBindingModel request)
        {
            return request.Nights <= 0 
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectNightsErrorMessage)
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        #endregion
    }
}
