using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Api.Controllers;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Tests
{
    public class CalendarControllerTests
    {
        #region Fields

        private Mock<ICalendarValidationService> _calendarValidationServiceMock;
        private Mock<ICalendarService> _calendarServiceMock;
        private CalendarController _calendarController;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _calendarServiceMock = new Mock<ICalendarService>();
            _calendarValidationServiceMock = new Mock<ICalendarValidationService>();
            _calendarController = new CalendarController(_calendarServiceMock.Object, _calendarValidationServiceMock.Object);
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public async Task Get_WhenGetCalendarByMissingRentalId_ThenReturns400BadRequestResponse()
        {
            // Arrange
            var rentalId = 1;
            var nights = 10;
            var startDate = DateTime.Now.Date.AddDays(10);

            _calendarValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetCalendarRequest>(y => y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.ValidationFailed });

            // Act
            var result = await _calendarController.Get(rentalId, startDate, nights);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var resp = result as ObjectResult;
            Assert.IsNotNull(resp);
            Assert.AreEqual(StatusCodes.Status400BadRequest, resp.StatusCode);
        }

        [Test]
        public async Task Get_WhenGetCalendarByCorrectParameters_ThenReturns200OkResponseAndCorrectResponseData()
        {
            // Arrange
            var rentalId = 1;
            var nights = 1;
            var startDate = DateTime.Now.Date.AddDays(10);

            var expectedResponseData = new CalendarViewModel
            {
                RentalId = 1,
                Dates = new List<CalendarDateViewModel>
                {
                     new CalendarDateViewModel
                     {
                         Date = startDate
                     }
                }
            };

            _calendarValidationServiceMock
                .Setup(x => x.ValidateGetRequest(It.Is<GetCalendarRequest>(y => y.RentalId == 1)))
                .Returns(new ServiceResponse<string> { Status = ResponseStatus.Success });

            _calendarServiceMock
                .Setup(x => x.GetAsync(It.Is<GetCalendarRequest>(y => y.RentalId == 1)))
                .ReturnsAsync(new ServiceResponse<CalendarViewModel> { Status = ResponseStatus.Success, Result = expectedResponseData });

            // Act
            var result = await _calendarController.Get(rentalId, startDate, nights);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);

            var actualResponseData = objectResult.Value as CalendarViewModel;
            Assert.AreSame(expectedResponseData, actualResponseData);
        }

        #endregion

        #endregion
    }
}