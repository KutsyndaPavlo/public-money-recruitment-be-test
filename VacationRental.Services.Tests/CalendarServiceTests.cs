using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Tests
{
    public class CalendarServiceTests
    {
        #region Fields

        private ICalendarService _calendarService;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _calendarService = new CalendarService(_unitOfWorkMock.Object);
        }

        #endregion

        #region Tests

        [Test]
        public async Task GetBookingsAsync_WhenRentalExists_ThenReturnsCorrectCalendarResult()
        {
            // Arrange
            var request = new GetCalendarRequest
            {
                RentalId = 1,
                StartDate = DateTime.Parse("2022-01-01"),
                Nights = 10
            };

            var rentalEntity = new RentalEntity
            { 
                Id = 1,
                PreparationTimeInDays = 2, 
                Units = 2
            };

            var bookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = 1,
                    Id = 10,
                    UnitId = 1,
                    BookingNights = 5,
                    BookingStart = DateTime.Parse("2022-01-01"),
                    BookingEnd = DateTime.Parse("2022-01-05"),
                    PreparationStart = DateTime.Parse("2022-01-06"),
                    PreparationEnd = DateTime.Parse("2022-01-07"),
                },
                new BookingEntity
                {
                    RentalId = 1,
                    Id = 11,
                    UnitId = 2,
                    BookingNights = 4,
                    BookingStart = DateTime.Parse("2022-01-03"),
                    BookingEnd = DateTime.Parse("2022-01-06"),
                    PreparationStart = DateTime.Parse("2022-01-07"),
                    PreparationEnd = DateTime.Parse("2022-01-08"),
                }
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetBookingsAsync(It.Is<int>(y => y == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _calendarService.GetAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<CalendarViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOf<CalendarViewModel>(result.Result);

            Assert.AreEqual(request.RentalId, result.Result.RentalId);
            Assert.AreEqual(request.Nights, result.Result.Dates.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-01"), result.Result.Dates[0].Date);
            Assert.AreEqual(1, result.Result.Dates[0].Bookings.Count);
            Assert.AreEqual(10, result.Result.Dates[0].Bookings[0].Id);
            Assert.AreEqual(1, result.Result.Dates[0].Bookings[0].Unit);
            Assert.AreEqual(0, result.Result.Dates[0].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-02"), result.Result.Dates[1].Date);
            Assert.AreEqual(1, result.Result.Dates[1].Bookings.Count);
            Assert.AreEqual(10, result.Result.Dates[1].Bookings[0].Id);
            Assert.AreEqual(1, result.Result.Dates[1].Bookings[0].Unit);
            Assert.AreEqual(0, result.Result.Dates[1].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-03"), result.Result.Dates[2].Date);
            Assert.AreEqual(2, result.Result.Dates[2].Bookings.Count);
            Assert.AreEqual(10, result.Result.Dates[2].Bookings[0].Id);
            Assert.AreEqual(1, result.Result.Dates[2].Bookings[0].Unit);
            Assert.AreEqual(11, result.Result.Dates[2].Bookings[1].Id);
            Assert.AreEqual(2, result.Result.Dates[2].Bookings[1].Unit);
            Assert.AreEqual(0, result.Result.Dates[2].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-04"), result.Result.Dates[3].Date);
            Assert.AreEqual(2, result.Result.Dates[3].Bookings.Count);
            Assert.AreEqual(10, result.Result.Dates[3].Bookings[0].Id);
            Assert.AreEqual(1, result.Result.Dates[3].Bookings[0].Unit);
            Assert.AreEqual(11, result.Result.Dates[3].Bookings[1].Id);
            Assert.AreEqual(2, result.Result.Dates[3].Bookings[1].Unit);
            Assert.AreEqual(0, result.Result.Dates[3].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-05"), result.Result.Dates[4].Date);
            Assert.AreEqual(2, result.Result.Dates[4].Bookings.Count);
            Assert.AreEqual(10, result.Result.Dates[4].Bookings[0].Id);
            Assert.AreEqual(1, result.Result.Dates[4].Bookings[0].Unit);
            Assert.AreEqual(11, result.Result.Dates[4].Bookings[1].Id);
            Assert.AreEqual(2, result.Result.Dates[4].Bookings[1].Unit);
            Assert.AreEqual(0, result.Result.Dates[4].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-06"), result.Result.Dates[5].Date);
            Assert.AreEqual(1, result.Result.Dates[5].Bookings.Count);
            Assert.AreEqual(11, result.Result.Dates[5].Bookings[0].Id);
            Assert.AreEqual(2, result.Result.Dates[5].Bookings[0].Unit);
            Assert.AreEqual(1, result.Result.Dates[5].PreparationTimes.Count);
            Assert.AreEqual(1, result.Result.Dates[5].PreparationTimes[0].Unit);

            Assert.AreEqual(DateTime.Parse("2022-01-07"), result.Result.Dates[6].Date);
            Assert.AreEqual(0, result.Result.Dates[6].Bookings.Count);
            Assert.AreEqual(2, result.Result.Dates[6].PreparationTimes.Count);
            Assert.AreEqual(1, result.Result.Dates[6].PreparationTimes[0].Unit);
            Assert.AreEqual(2, result.Result.Dates[6].PreparationTimes[1].Unit);

            Assert.AreEqual(DateTime.Parse("2022-01-08"), result.Result.Dates[7].Date);
            Assert.AreEqual(0, result.Result.Dates[7].Bookings.Count);
            Assert.AreEqual(1, result.Result.Dates[7].PreparationTimes.Count);
            Assert.AreEqual(2, result.Result.Dates[7].PreparationTimes[0].Unit);

            Assert.AreEqual(DateTime.Parse("2022-01-09"), result.Result.Dates[8].Date);
            Assert.AreEqual(0, result.Result.Dates[8].Bookings.Count);
            Assert.AreEqual(0, result.Result.Dates[8].PreparationTimes.Count);

            Assert.AreEqual(DateTime.Parse("2022-01-10"), result.Result.Dates[9].Date);
            Assert.AreEqual(0, result.Result.Dates[9].Bookings.Count);
            Assert.AreEqual(0, result.Result.Dates[9].PreparationTimes.Count);
        }

        #endregion
    }
}