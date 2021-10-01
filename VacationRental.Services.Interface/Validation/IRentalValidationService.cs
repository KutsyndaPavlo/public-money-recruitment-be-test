using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface.Validation
{
    public interface IRentalValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetRentalRequest request);

        ServiceResponse<string> ValidatePostRequest(RentalBindingModel request);

        ServiceResponse<string> ValidatePutRequest(PutRentalRequest request);
    }
}
