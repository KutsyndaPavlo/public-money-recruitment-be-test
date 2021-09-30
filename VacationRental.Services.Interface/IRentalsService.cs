using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IRentalsService
    {
        ServiceResponse<ResourceIdViewModel> Add(RentalBindingModel request);

        ServiceResponse<RentalViewModel> GetById(GetRentalRequest request);

        ServiceResponse<ResourceIdViewModel> Update(PutRentalRequest request);
    }
}