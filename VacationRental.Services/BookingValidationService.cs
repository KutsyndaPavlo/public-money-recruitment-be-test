using VacationRental.Dal.Interface;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class BookingValidationService : IBookingValidatinService
    {
        #region Fields

        private const string incorrectBookingIdErrorMessage = "Incorrect booking id";
        private const string incorrectNightsErrorMessage = "Nigts must be positive";
        private const string rentalNotFoundErrorMessage = "Rental not found";
        private readonly IRentalsRepository _rentalRepository;

        #endregion

        #region Fields

        public BookingValidationService(IRentalsRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        #endregion

        #region Methods

        public ServiceResponse<string> ValidateGetRequest(GetBookingRequest request)
        {
            if (request.BookingId <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectBookingIdErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        public ServiceResponse<string> ValidatePostRequest(BookingBindingModel request)
        {
            if (request.Nights <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectNightsErrorMessage };
            }

            if (_rentalRepository.GetById(request.RentalId) == null)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = rentalNotFoundErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
