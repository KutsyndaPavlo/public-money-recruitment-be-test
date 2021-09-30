using VacationRental.Api.Models;
using VacationRental.Services.Models;

namespace VacationRental.Services
{
    public interface IBookingsService
    {
        ServiceResponse<BookingViewModel> Get(int id);

        ServiceResponse<ResourceIdViewModel> Add(BookingBindingModel rentalEntityCreate);
    }
}