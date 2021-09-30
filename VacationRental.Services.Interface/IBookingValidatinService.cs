using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IBookingValidatinService
    {
        ServiceResponse<string> ValidateGetRequest(GetBookingRequest request);

        ServiceResponse<string> ValidatePostRequest(BookingBindingModel model);
    }
}
