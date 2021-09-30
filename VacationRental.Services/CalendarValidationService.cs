using VacationRental.Dal.Interface;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class CalendarValidationService : ICalendarValidationService
    {
        #region Fields

        private const string incorrectNightsErrorMessage = "Nigts must be positive";
        private const string rentalNotFoundErrorMessage = "Rental not found";
        private readonly IRentalsRepository _rentalRepository;

        #endregion

        #region Fields

        public CalendarValidationService(IRentalsRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        #endregion

        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetCalendarRequest request)
        {
            if (_rentalRepository.GetById(request.RentalId) == null)
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
