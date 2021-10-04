using NUnit.Framework;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;
using VacationRental.Services.Validation;

namespace VacationRental.Services.Tests.Validation
{
    public class CalendarValidationServiceTests
    {
        #region Fields

        private ICalendarValidationService _calendarValidationService;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _calendarValidationService = new CalendarValidationService();
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public void ValidateGetRequest_WhenValidateGetCalendarRequestWithNotValidNights_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new GetCalendarRequest
            {
                Nights = 0
            };

            // Act
            var result = _calendarValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectNightsErrorMessage, result.Result);
        }

        [Test]
        public void ValidateGetRequest_WhenValidateGetCalendarRequestWithValidParameters_ThenReturnsSuccessResult()
        {
            // Arrange
            var request = new GetCalendarRequest
            {
                Nights = 11
            };

            // Act
            var result = _calendarValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #endregion
    }
}
