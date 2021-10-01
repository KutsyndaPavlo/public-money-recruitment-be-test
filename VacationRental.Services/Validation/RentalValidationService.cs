using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class RentalValidationService : IRentalValidationService
    {
        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetRentalRequest request)
        {
            if (request.RentalId <= 0)
            {
                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed, 
                    Result = VacationRentalConstants.IncorrectRentalIdErrorMessage
                };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePostRequest(RentalBindingModel request)
        {
            if (request.Units <= 0)
            {
                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed, 
                    Result = VacationRentalConstants.IncorrectUnitsErrorMessage
                };
            }

            if (request.PreparationTimeInDays <= 0)
            {
                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed,
                    Result = VacationRentalConstants.IncorrectPreparationTimeErrorMessage
                };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePutRequest(PutRentalRequest request)
        {
            if (request.Units <= 0)
            {
                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed, 
                    Result = VacationRentalConstants.IncorrectUnitsErrorMessage
                };
            }

            if (request.PreparationTimeInDays < 0)
            {
                return new ServiceResponse<string>
                {
                    Status = ResponseStatus.ValidationFailed, 
                    Result = VacationRentalConstants.IncorrectPreparationTimeErrorMessage
                };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
