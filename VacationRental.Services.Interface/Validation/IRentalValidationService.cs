using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Interface.Validation
{
    public interface IRentalValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetRentalRequest request);

        ServiceResponse<string> ValidatePostRequest(RentalBindingModel request);

        ServiceResponse<string> ValidatePutRequest(PutRentalRequest request);
    }
}
