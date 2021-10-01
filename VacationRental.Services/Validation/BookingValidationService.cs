using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class BookingValidationService : IBookingValidatinService
    {
        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetBookingRequest request)
        {
            return request.BookingId <= 0 
                ? new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed, 
                    Result = VacationRentalConstants.IncorrectBookingIdErrorMessage
                } 
                : new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePostRequest(BookingBindingModel request)
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
