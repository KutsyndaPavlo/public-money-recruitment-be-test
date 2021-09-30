using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IBookingsService
    {
        ServiceResponse<BookingViewModel> Get(int id);

        ServiceResponse<ResourceIdViewModel> Add(BookingBindingModel rentalEntityCreate);
    }
}