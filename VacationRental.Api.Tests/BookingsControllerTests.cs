using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using VacationRental.Api.Controllers;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Tests
{

    public class BookingsControllerTests
    {
        #region Fields

        private Mock<IBookingValidationService> _bookingValidationServiceMock;
        private Mock<IBookingsService> _bookingsServiceMock;
        private BookingsController _bookingsController;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _bookingsServiceMock = new Mock<IBookingsService>();
            _bookingValidationServiceMock = new Mock<IBookingValidationService>();
            _bookingsController = new BookingsController(_bookingsServiceMock.Object, _bookingValidationServiceMock.Object);
        }

        #endregion

        #region Tests

        [Test]
        public async Task Get_WhenGetBookingByIncorrectId_ThenReturnsBadRequestResponse()
        {
            // Arrange
            var bookingId = -5;
            _bookingValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetBookingRequest>(y => y.BookingId == -5)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _bookingsController.Get(bookingId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Get_WhenGetBookingByMissingBookingId_ThenReturnNotFoundResponse()
        {
            // Arrange
            var bookingId = 999999999;
            _bookingValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetBookingRequest>(y => y.BookingId == 999999999)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _bookingsServiceMock
                .Setup(x => x.GetAsync(It.Is<GetBookingRequest>(y => y.BookingId == 999999999)))
                .ReturnsAsync(new ServiceResponse<BookingViewModel> { Status = ResponseStatus.BookingNotFound });

            // Act
            var result = await _bookingsController.Get(bookingId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status404NotFound, resp.StatusCode);
        }

        [Test]
        public async Task Get_WhenGetBookingByCorrectBookingId_ThenReturnOkResponseAndCorrectResponseData()
        {
            // Arrange
            var bookingId = 1;
            var expectedBooking = new BookingViewModel { Id = bookingId };
            _bookingValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetBookingRequest>(y => y.BookingId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _bookingsServiceMock
                .Setup(x => x.GetAsync(It.Is<GetBookingRequest>(y => y.BookingId == 1)))
                .ReturnsAsync(new ServiceResponse<BookingViewModel> { Status = ResponseStatus.Success, Result = expectedBooking});

            // Act
            var response = await _bookingsController.Get(bookingId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<IActionResult>(response);

            var objectResult = response as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);

            var actualBooking = objectResult.Value as BookingViewModel;
            Assert.AreSame(expectedBooking, actualBooking);
        }

        #endregion
    }
}
