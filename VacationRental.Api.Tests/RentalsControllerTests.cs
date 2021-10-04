using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VacationRental.Api.Controllers;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
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

        [Test]
        public async Task Get_WhenGetRentalByIncorrectId_ThenReturnsBadRequestResponse()
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
        public async Task Get_WhenGetRentalByMissingRentalId_ThenReturnNotFoundResponse()
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
        public async Task Get_WhenGetRentalByCorrectRentalId_ThenReturnOkResponseAndCorrectResponseData()
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
    }
}
