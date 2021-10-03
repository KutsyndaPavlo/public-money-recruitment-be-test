using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class RentalValidationService : ServiceBase, IRentalValidationService
    {
        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetRentalRequest request)
        {
            return request.RentalId <= 0 
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectRentalIdErrorMessage)
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        public ServiceResponse<string> ValidatePostRequest(RentalBindingModel request)
        {
            if (request.Units <= 0)
            {
                return GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectUnitsErrorMessage);
            }

            return request.PreparationTimeInDays <= 0 
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectPreparationTimeErrorMessage) 
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        public ServiceResponse<string> ValidatePutRequest(PutRentalRequest request)
        {
            if (request.Units <= 0)
            {
                return GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectUnitsErrorMessage);
            }

            return request.PreparationTimeInDays < 0 
                ? GetServiceResponse(ResponseStatus.ValidationFailed, VacationRentalConstants.IncorrectPreparationTimeErrorMessage) 
                : GetServiceResponse<string>(ResponseStatus.Success);
        }

        #endregion
    }
}
