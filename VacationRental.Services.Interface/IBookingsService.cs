using System.Threading.Tasks;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface
{
    public interface IBookingsService
    {
        Task<ServiceResponse<BookingViewModel>> GetAsync(GetBookingRequest request);

        Task<ServiceResponse<ResourceIdViewModel>> AddAsync(BookingBindingModel request);
    }
}