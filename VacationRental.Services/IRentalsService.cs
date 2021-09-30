using VacationRental.Api.Models;
using VacationRental.Data.Entities;
using VacationRental.Services.Models;

namespace VacationRental.Services
{
    public interface IRentalsService
    {
        ServiceResponse<ResourceIdViewModel> Add(RentalBindingModel units);

        ServiceResponse<RentalViewModel> GetById(int id);

        ServiceResponse<ResourceIdViewModel> Update(int id, RentalBindingModel units);
    }
}