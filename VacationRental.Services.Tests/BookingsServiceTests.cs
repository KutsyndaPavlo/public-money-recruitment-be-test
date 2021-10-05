using AutoMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Tests
{
    public class BookingsServiceTests
    {
        #region Fields

        private IBookingsService _bookingService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _bookingService = new BookingsService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public async Task GetByIdAsync_WhenRentalIsMissing_ThenReturnsRentalNotFoundResult()
        {
            // Arrange
            var request = new GetBookingRequest
            {
                BookingId = 1
            };

            BookingEntity bookingEntity = null;

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(bookingEntity);

            // Act
            var result = await _bookingService.GetAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<BookingViewModel>>(result);
            Assert.AreEqual(ResponseStatus.BookingNotFound, result.Status);
        }

        [Test]
        public async Task GetByIdAsync_WhenRentalExists_ThenReturnsRental()
        {
            // Arrange
            var request = new GetBookingRequest
            {
                BookingId = 1
            };

            var bookingEntity = new BookingEntity
            {
                Id = 5,
                RentalId = 1,
                BookingNights = 10
            };

            var bookingViewModel = new BookingViewModel
            {
                Id = 5,
                RentalId = 1,
                Nights = 10
            };

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(bookingEntity);

            _mapperMock.Setup(x => x.Map<BookingViewModel>(It.Is<BookingEntity>(y => y.Id == 5)))
                .Returns(bookingViewModel);

            // Act
            var result = await _bookingService.GetAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<BookingViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreSame(bookingViewModel, result.Result);
        }

        #endregion

        #region Add

        [Test]
        public async Task AddAsync_WhenRentalIsMissing_ThenReturnsRentalNotFoundResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                RentalId = 1,
                Nights = 5,
                Start = DateTime.Parse("2022-06-01")
            };

            RentalEntity rentalEntity = null;

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            // Act
            var result = await _bookingService.AddAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<ResourceIdViewModel>>(result);
            Assert.AreEqual(ResponseStatus.RentalNotFound, result.Status);
        }

        [Test]
        public async Task AddAsync_WhenThereAreNoOverlappedBookings_ThenReturnsCreatedBookingResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                RentalId = 10,
                Nights = 5,
                Start = DateTime.Parse("2022-02-01")
            };

            var rentalEntity = new RentalEntity
            {
                Id = request.RentalId,
                Units = 1,
                PreparationTimeInDays = 2
            };

            var availableBookings = new List<BookingEntity>();

            var bookingEntityCreate = new BookingEntityCreate
            {
                RentalId = rentalEntity.Id,
                UnitId = 1,
                Nights = request.Nights,
                Start = request.Start
            };

            var createdBooking = new BookingEntity
            {
                RentalId = bookingEntityCreate.RentalId,
                Id = 14,
                UnitId = bookingEntityCreate.UnitId,
                BookingNights = bookingEntityCreate.Nights,
                BookingStart = bookingEntityCreate.Start,
                BookingEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights - 1),
                PreparationStart = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights),
                PreparationEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights + rentalEntity.PreparationTimeInDays - 1)
            };

            var expectedResponseData = new ResourceIdViewModel
            {
                Id = createdBooking.Id
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 10)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetBookingsAsync(
                    It.Is<int>(y => y == 10), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.AddAsync(
                    It.Is<BookingEntityCreate>(y => y.RentalId == 10 && y.UnitId == 1)))
                .ReturnsAsync(createdBooking);

            _mapperMock.Setup(x => x.Map<BookingEntityCreate>(It.Is<BookingBindingModel>(
                y => y.RentalId == 10))).Returns(bookingEntityCreate);

            _mapperMock.Setup(x => x.Map<ResourceIdViewModel>(It.Is<BookingEntity>(
                y => y.Id == 14))).Returns(expectedResponseData);

            // Act
            var result = await _bookingService.AddAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<ResourceIdViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreSame(expectedResponseData, result.Result);
        }

        [Test]
        public async Task AddAsync_WhenThereAreAdditionalAvailableUnit_ThenReturnsCreatedBookingResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                RentalId = 10,
                Nights = 5,
                Start = DateTime.Parse("2022-02-01")
            };

            var rentalEntity = new RentalEntity
            {
                Id = request.RentalId,
                Units = 2,
                PreparationTimeInDays = 2
            };

            var bookingEntityCreate = new BookingEntityCreate
            {
                RentalId = rentalEntity.Id,
                Nights = request.Nights,
                Start = request.Start
            };

            var availableBookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = bookingEntityCreate.RentalId,
                    Id = 9,
                    UnitId = 1,
                    BookingNights = bookingEntityCreate.Nights,
                    BookingStart = bookingEntityCreate.Start,
                    BookingEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights - 1),
                    PreparationStart = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights),
                    PreparationEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights + rentalEntity.PreparationTimeInDays - 1)
                }
            };

            var createdBooking = new BookingEntity
            {
                RentalId = bookingEntityCreate.RentalId,
                Id = 14,
                UnitId = 2,
                BookingNights = bookingEntityCreate.Nights,
                BookingStart = bookingEntityCreate.Start,
                BookingEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights - 1),
                PreparationStart = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights),
                PreparationEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights + rentalEntity.PreparationTimeInDays - 1)
            };

            var expectedResponseData = new ResourceIdViewModel
            {
                Id = createdBooking.Id
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 10)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetBookingsAsync(
                    It.Is<int>(y => y == 10), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.AddAsync(
                    It.Is<BookingEntityCreate>(y => y.RentalId == 10 && y.UnitId == 2)))
                .ReturnsAsync(createdBooking);

            _mapperMock.Setup(x => x.Map<BookingEntityCreate>(It.Is<BookingBindingModel>(
                y => y.RentalId == 10))).Returns(bookingEntityCreate);

            _mapperMock.Setup(x => x.Map<ResourceIdViewModel>(It.Is<BookingEntity>(
                y => y.Id == 14))).Returns(expectedResponseData);

            // Act
            var result = await _bookingService.AddAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<ResourceIdViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreSame(expectedResponseData, result.Result);
        }

        [Test]
        public async Task GetByIdAsync_WhenThereAreBookingOverBooking_ThenReturnsConflictResult()
        {
            // Arrange
            var request = new BookingBindingModel
            {
                RentalId = 10,
                Nights = 5,
                Start = DateTime.Parse("2022-02-01")
            };

            var rentalEntity = new RentalEntity
            {
                Id = 10,
                Units = 1,
                PreparationTimeInDays = 2
            };

            var bookingEntityCreate = new BookingEntityCreate
            {
                RentalId = rentalEntity.Id,
                Nights = request.Nights,
                Start = request.Start
            };

            var availableBookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = bookingEntityCreate.RentalId,
                    Id = 9,
                    UnitId = 1,
                    BookingNights = bookingEntityCreate.Nights,
                    BookingStart = bookingEntityCreate.Start,
                    BookingEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights - 1),
                    PreparationStart = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights),
                    PreparationEnd = bookingEntityCreate.Start.AddDays(bookingEntityCreate.Nights + rentalEntity.PreparationTimeInDays - 1)
                }
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 10)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository.GetBookingsAsync(
                    It.Is<int>(y => y == 10), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            // Act
            var result = await _bookingService.AddAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<ResourceIdViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Conflict, result.Status);
        }

        #endregion

        #endregion
    }
}
