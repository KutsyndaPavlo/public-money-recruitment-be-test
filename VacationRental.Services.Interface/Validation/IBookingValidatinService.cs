using System.Threading.Tasks;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface.Validation
{
    public interface IBookingValidatinService
    {
        ServiceResponse<string> ValidateGetRequest(GetBookingRequest request);

        ServiceResponse<string> ValidatePostRequest(BookingBindingModel request);
    }
}
