using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
{
    public class CalendarValidationService : ICalendarValidationService
    {
        #region Fields

        private const string incorrectNightsErrorMessage = "Nigts must be positive";
        private const string rentalNotFoundErrorMessage = "Rental not found";
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Fields

        public CalendarValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse<string>> ValidateGetRequestAsync(GetCalendarRequest request)
        {
            if (await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId) == null)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = rentalNotFoundErrorMessage };
            }

            if (request.Nights <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectNightsErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
