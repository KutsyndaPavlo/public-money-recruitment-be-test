using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Services.Validation
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

        public async Task<ServiceResponse<string>> ValidatePostRequestAsync(BookingBindingModel request)
        {
            if (request.Nights <= 0)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = incorrectNightsErrorMessage };
            }

            if (await _rentalRepository.GetByIdAsync(request.RentalId) == null)
            {
                return new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed, Result = rentalNotFoundErrorMessage };
            }

            return new ServiceResponse<string> { Status = ResponseStatus.Success };
        }

        #endregion
    }
}
