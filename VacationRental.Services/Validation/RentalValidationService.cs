using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class RentalValidationService : IRentalValidationService
    {
        #region Fields

        private const string incorrectRentalIdErrorMessage = "Incorrect rental id";
        private const string incorrectUnitsErrorMessage = "Units must be positive";
        private const string incorrectPreparationTimeErrorMessage = "Units must be zero or positive";

        #endregion

        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetRentalRequest request)
        {
            if (request.RentalId <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectRentalIdErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePostRequest(RentalBindingModel request)
        {
            if (request.Units <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectUnitsErrorMessage };
            }

            if (request.PreparationTimeInDays < 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectPreparationTimeErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePutRequest(PutRentalRequest request)
        {
            if (request.Units <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectUnitsErrorMessage };
            }

            if (request.PreparationTimeInDays < 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectPreparationTimeErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
