using NUnit.Framework;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;
using VacationRental.Services.Validation;

namespace VacationRental.Services.Tests.Validation
{
    public class RentalValidationServiceTests
    {
        #region Fields

        private IRentalValidationService _rentalValidationService;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _rentalValidationService = new RentalValidationService();
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public void ValidateGetRequest_WhenValidateGetRentalRequestWithNotValidId_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new GetRentalRequest
            {
                RentalId = 0
            };

            // Act
            var result = _rentalValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectRentalIdErrorMessage, result.Result);
        }

        [Test]
        public void ValidateGetRequest_WhenValidateGetRentalRequestWithValidId_ThenReturnsSuccessResult()
        {
            // Arrange
            var request = new GetRentalRequest
            {
                RentalId = 1
            };

            // Act
            var result = _rentalValidationService.ValidateGetRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #region Post

        [Test]
        public void ValidatePostRequest_WhenValidatePostRentalRequestWithNotValidNights_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new RentalBindingModel
            {
                Units = 0,
                PreparationTimeInDays = 5
            };

            // Act
            var result = _rentalValidationService.ValidatePostRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectUnitsErrorMessage, result.Result);
        }

        [Test]
        public void ValidatePostRequest_WhenValidatePostRentalRequestWithNotValidPreparationTimeInDays_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new RentalBindingModel
            {
                Units = 10,
                PreparationTimeInDays = 0
            };

            // Act
            var result = _rentalValidationService.ValidatePostRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectPreparationTimeErrorMessage, result.Result);
        }

        [Test]
        public void ValidatePostRequest_WhenValidatePostRentalRequestWithValidParameters_ThenReturnsSuccessResult()
        {
            // Arrange
            var request = new RentalBindingModel
            {
                Units = 10,
                PreparationTimeInDays = 10
            };

            // Act
            var result = _rentalValidationService.ValidatePostRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #region Put

        [Test]
        public void ValidatePutRequest_WhenValidatePutRentalRequestWithNotValidNights_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                Units = 0,
                PreparationTimeInDays = 5
            };

            // Act
            var result = _rentalValidationService.ValidatePutRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectUnitsErrorMessage, result.Result);
        }

        [Test]
        public void ValidatePutRequest_WhenValidatePutRentalRequestWithNotValidPreparationTimeInDays_ThenReturnsValidationFailedResult()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                Units = 10,
                PreparationTimeInDays = 0
            };

            // Act
            var result = _rentalValidationService.ValidatePutRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.ValidationFailed, result.Status);
            Assert.AreEqual(VacationRentalConstants.IncorrectPreparationTimeErrorMessage, result.Result);
        }

        [Test]
        public void ValidatePutRequest_WhenValidatePutRentalRequestWithValidParameters_ThenReturnsSuccessResult()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                Units = 10,
                PreparationTimeInDays = 10
            };

            // Act
            var result = _rentalValidationService.ValidatePutRequest(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<string>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
        }

        #endregion

        #endregion
    }
}
