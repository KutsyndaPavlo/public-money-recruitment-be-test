using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface
{
    public interface IRentalValidationService
    {
        ServiceResponse<string> ValidateGetRequest(GetRentalRequest request);

        ServiceResponse<string> ValidatePostRequest(RentalBindingModel request);

        ServiceResponse<string> ValidatePutRequest(PutRentalRequest request);
    }
}
