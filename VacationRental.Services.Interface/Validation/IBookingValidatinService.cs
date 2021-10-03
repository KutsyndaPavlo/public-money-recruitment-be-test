using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface.Validation
{
    public interface IBookingValidatinService
    {
        ServiceResponse<string> ValidateGetRequest(GetBookingRequest request);

        ServiceResponse<string> ValidatePostRequest(BookingBindingModel request);
    }
}
