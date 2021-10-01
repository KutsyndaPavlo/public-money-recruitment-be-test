using System.Threading.Tasks;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IBookingValidatinService
    {
        ServiceResponse<string> ValidateGetRequest(GetBookingRequest request);

        Task<ServiceResponse<string>> ValidatePostRequestAsync(BookingBindingModel request);
    }
}
