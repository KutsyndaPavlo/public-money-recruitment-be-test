using System.Threading.Tasks;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface
{
    public interface IRentalsService
    {
        Task<ServiceResponse<ResourceIdViewModel>> AddAsync(RentalBindingModel request);

        Task<ServiceResponse<RentalViewModel>> GetByIdAsync(GetRentalRequest request);

        Task<ServiceResponse<ResourceIdViewModel>> UpdateAsync(PutRentalRequest request);
    }
}