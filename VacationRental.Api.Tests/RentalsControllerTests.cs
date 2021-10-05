using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using VacationRental.Api.Controllers;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Tests
{
    [TestFixture]
    public class RentalsControllerTests
    {
        #region Fields

        private Mock<IRentalValidationService> _rentalValidationServiceMock;
        private Mock<IRentalsService> _rentalsServiceMock;
        private RentalsController _rentalsController;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _rentalsServiceMock = new Mock<IRentalsService>();
            _rentalValidationServiceMock = new Mock<IRentalValidationService>();
            _rentalsController = new RentalsController(_rentalsServiceMock.Object, _rentalValidationServiceMock.Object);
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public async Task Get_WhenGetRentalByIncorrectId_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rentalId = -5;
            _rentalValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetRentalRequest>(y => y.RentalId == -5)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _rentalsController.Get(rentalId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Get_WhenGetRentalByMissingRentalId_ThenReturns404NotFoundResponse()
        {
            // Arrange
            var rentalId = 999999999;
            _rentalValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetRentalRequest>(y => y.RentalId == 999999999)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.GetByIdAsync(It.Is<GetRentalRequest>(y => y.RentalId == 999999999)))
                .ReturnsAsync(new ServiceResponse<RentalViewModel> { Status = ResponseStatus.RentalNotFound });

            // Act
            var result = await _rentalsController.Get(rentalId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status404NotFound, resp.StatusCode);
        }

        [Test]
        public async Task Get_WhenGetRentalByCorrectRentalId_ThenReturns200OkResponseAndCorrectResponseData()
        {
            // Arrange
            var rentalId = 1;
            var expectedRental = new RentalViewModel
            {
                Id = rentalId,
                Units = 10,
                PreparationTimeInDays = 2
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetRentalRequest>(y => y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.GetByIdAsync(It.Is<GetRentalRequest>(y => y.RentalId == 1)))
                .ReturnsAsync(new ServiceResponse<RentalViewModel> { Status = ResponseStatus.Success, Result = expectedRental });

            // Act
            var result = await _rentalsController.Get(rentalId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);

            var actualRental = objectResult.Value as RentalViewModel;
            Assert.AreSame(expectedRental, actualRental);
        }

        #endregion

        #region Post

        [Test]
        public async Task Post_WhenPostRentalWithIncorrectPreparationTimeInDays_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 0,
                Units = 10
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePostRequest(It.Is<RentalBindingModel>(y => y.PreparationTimeInDays == 0)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _rentalsController.Post(rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Post_WhenPostRentalWithIncorrectUnits_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 0
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePostRequest(It.Is<RentalBindingModel>(y => y.Units == 0)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _rentalsController.Post(rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Post_WhenPostRentalWithCorrectParameters_ThenReturns201CreatedResponseAndCorrectResponseData()
        {
            // Arrange
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 10
            };

            var expectedResponseData = new ResourceIdViewModel { Id = 1 };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePostRequest(It.Is<RentalBindingModel>(
                    y => y.Units == 10 && y.PreparationTimeInDays == 10)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.AddAsync(It.Is<RentalBindingModel>(y =>
                    y.Units == 10 && y.PreparationTimeInDays == 10)))
                .ReturnsAsync(new ServiceResponse<ResourceIdViewModel> { Status = ResponseStatus.Success, Result = expectedResponseData });

            // Act
            var result = await _rentalsController.Post(rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);

            var actualRental = objectResult.Value as ResourceIdViewModel;
            Assert.AreSame(expectedResponseData, actualRental);
        }

        #endregion

        #region Put

        [Test]
        public async Task Put_WhenUpdatingRentalWithIncorrectPreparationTimeInDays_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rentalId = 1;
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 0,
                Units = 10
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePutRequest(It.Is<PutRentalRequest>(
                    y => y.RentalId == 1 && y.PreparationTimeInDays == 0 && y.Units == 10)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _rentalsController.Put(rentalId, rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Put_WhenUpdatingRentalWithIncorrectUnits_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rentalId = 1;
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 0
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePutRequest(
                    It.Is<PutRentalRequest>(y => y.Units == 0 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _rentalsController.Put(rentalId, rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Put_WhenUpdatingRentalWithMissingRentalId_ThenReturns404NotFoundResponse()
        {
            // Arrange
            var rentalId = 1;
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 10
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePutRequest(
                    It.Is<PutRentalRequest>(y => y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.UpdateAsync(It.Is<PutRentalRequest>(y =>
                    y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .ReturnsAsync(new ServiceResponse<RentalViewModel> { Status = ResponseStatus.RentalNotFound });

            // Act
            var result = await _rentalsController.Put(rentalId, rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status404NotFound, resp.StatusCode);
        }

        [Test]
        public async Task Put_WhenUpdatingRentalAndThereIsOverBookings_ThenReturns409ConflictResponse()
        {
            // Arrange
            var rentalId = 1;
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 10
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePutRequest(
                    It.Is<PutRentalRequest>(y => y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.UpdateAsync(It.Is<PutRentalRequest>(y =>
                    y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .ReturnsAsync(new ServiceResponse<RentalViewModel> { Status = ResponseStatus.Conflict });

            // Act
            var result = await _rentalsController.Put(rentalId, rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status409Conflict, resp.StatusCode);
        }

        [Test]
        public async Task Put_WhenUpdatingRentalWithCorrectParameters_ThenReturns200OkResponseAndCorrectResponseData()
        {
            // Arrange
            var rentalId = 1;
            var rental = new RentalBindingModel
            {
                PreparationTimeInDays = 10,
                Units = 10
            };

            var expectedResponseData = new RentalViewModel
            {
                 Id = 1,
                 PreparationTimeInDays = 10,
                 Units = 10
            };

            _rentalValidationServiceMock
                .Setup(x => x.ValidatePutRequest(
                    It.Is<PutRentalRequest>(y => y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _rentalsServiceMock
                .Setup(x => x.UpdateAsync(It.Is<PutRentalRequest>(y =>
                    y.Units == 10 && y.PreparationTimeInDays == 10 && y.RentalId == 1)))
                .ReturnsAsync(new ServiceResponse<RentalViewModel> { Status = ResponseStatus.Success, Result = expectedResponseData });

            // Act
            var result = await _rentalsController.Put(rentalId, rental);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);

            var actualRental = objectResult.Value as RentalViewModel;
            Assert.AreSame(expectedResponseData, actualRental);
        }

        #endregion

        #endregion
    }
}
