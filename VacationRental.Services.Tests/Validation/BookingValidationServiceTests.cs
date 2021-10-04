using NUnit.Framework;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;
using VacationRental.Services.Validation;

namespace VacationRental.Services.Tests.Validation
{
    public class BookingValidationServiceTests
    {
        #region Fields

        private IBookingValidationService _bookingValidationService;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _bookingValidationService = new BookingValidationService();
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public void ValidateGetRequest_WhenValidateGetBookingRequestWithNotValidId_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new GetBookingRequest
            {
                BookingId = 0
            };

            // Act
            var result = _bookingValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectBookingIdErrorMessage, result.Result);
        }

        [Test]
        public void ValidateGetRequest_WhenValidateGetBookingRequestWithValidId_ThenReturnsSuccessResult()
        {
            // Arrange
            var request = new GetBookingRequest
            {
                BookingId = 1
            };

            // Act
            var result = _bookingValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #region Post

        [Test]
        public void ValidatePostRequest_WhenValidatePostBookingRequestWithNotValidNights_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                Nights = 0
            };

            // Act
            var result = _bookingValidationService.ValidatePostRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectNightsErrorMessage, result.Result);
        }


        [Test]
        public void ValidatePostRequest_WhenValidatePostBookingRequestWithValidNights_ThenReturnsSucceessResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                Nights = 10
            };

            // Act
            var result = _bookingValidationService.ValidatePostRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #endregion
    }
}
