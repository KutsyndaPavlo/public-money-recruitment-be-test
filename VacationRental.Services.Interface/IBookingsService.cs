using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IBookingsService
    {
        ServiceResponse<BookingViewModel> Get(GetBookingRequest request);

        ServiceResponse<ResourceIdViewModel> Add(BookingBindingModel request);
    }
}