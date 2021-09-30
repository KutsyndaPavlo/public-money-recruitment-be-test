using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IRentalsService
    {
        ServiceResponse<ResourceIdViewModel> Add(RentalBindingModel units);

        ServiceResponse<RentalViewModel> GetById(int id);

        ServiceResponse<ResourceIdViewModel> Update(int id, RentalBindingModel units);
    }
}