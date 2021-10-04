using AutoMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services.Tests
{
    public class RentalsServiceTests
    {
        #region Fields

        private IRentalsService _rentalsService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _rentalsService = new RentalsService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #endregion

        #region Tests

        #region Get

        [Test]
        public async Task GetByIdAsync_WhenRentalIsMissing_ThenReturnsRentalNotFoundResult()
        {
            // Arrange
            var request = new GetRentalRequest
            {
                RentalId = 1
            };

            RentalEntity rentalEntity = null;

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            // Act
            var result = await _rentalsService.GetByIdAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.RentalNotFound, result.Status);
        }

        [Test]
        public async Task GetByIdAsync_WhenRentalExists_ThenReturnsRental()
        {
            // Arrange
            var request = new GetRentalRequest
            {
                RentalId = 1
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                Units = 5,
                PreparationTimeInDays = 2
            };

            var rentalViewModel = new RentalViewModel
            {
                Id = 1,
                Units = 5,
                PreparationTimeInDays = 2
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository.GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _mapperMock.Setup(x => x.Map<RentalViewModel>(It.Is<RentalEntity>(y => y.Id == 1 && y.Units == 5 && y.PreparationTimeInDays == 2)))
                .Returns(rentalViewModel);

            // Act
            var result = await _rentalsService.GetByIdAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreSame(rentalViewModel, result.Result);
        }

        #endregion

        #region Post

        [Test]
        public async Task AddAsync_WhenRentalDataAreValid_ThenReturnsResourceIdViewModel()
        {
            // Arrange
            var request = new RentalBindingModel
            {
                Units = 5,
                PreparationTimeInDays = 2
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 5
            };

            var rentalEntityCreate = new RentalEntityCreate
            {
                PreparationTimeInDays = 2,
                Units = 5
            };

            var resourceId = new ResourceIdViewModel
            {
                Id = 1
            };

            _unitOfWorkMock
            .Setup(x => x.RentalsRepository
                .AddAsync(It.Is<RentalEntityCreate>(y => y.Units == 5 && y.PreparationTimeInDays == 2)))
                .ReturnsAsync(rentalEntity);

            _mapperMock.Setup(x =>
                    x.Map<RentalEntityCreate>(It.Is<RentalBindingModel>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2)))
                .Returns(rentalEntityCreate);

            _mapperMock.Setup(x =>
                    x.Map<ResourceIdViewModel>(It.Is<RentalEntity>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .Returns(resourceId);

            // Act
            var result = await _rentalsService.AddAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<ResourceIdViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreEqual(resourceId, result.Result);
        }

        #endregion

        #region Put

        [Test]
        public async Task UpdateAsync_WhenRentalIsMissing_ThenReturnsRentalNotFoundResponse()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                RentalId = 1,
                Units = 5,
                PreparationTimeInDays = 2
            };

            RentalEntity rentalEntity = null;

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            // Act
            var result = await _rentalsService.UpdateAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.RentalNotFound, result.Status);
        }

        [Test]
        public async Task UpdateAsync_WhenThereAreNoBookingOverBooking_ThenReturnsUpdatedRental()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                RentalId = 1,
                Units = 5,
                PreparationTimeInDays = 2
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 6
            };

            var updatedRentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 5
            };

            var expectedResult = new RentalViewModel
            {
                Id = request.RentalId,
                Units = request.Units,
                PreparationTimeInDays = request.PreparationTimeInDays
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository
                    .GetBookingsAsync(It.Is<int>(y => y == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<BookingEntity>());

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .UpdateAsync(It.Is<RentalEntity>(y => y.Units == 5 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .ReturnsAsync(updatedRentalEntity);

            _mapperMock.Setup(x =>
                    x.Map<RentalEntity>(It.Is<PutRentalRequest>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2)))
                .Returns(updatedRentalEntity);

            _mapperMock.Setup(x =>
                    x.Map<RentalViewModel>(It.Is<RentalEntity>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .Returns(expectedResult);

            // Act
            var result = await _rentalsService.UpdateAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreEqual(expectedResult, result.Result);

            _unitOfWorkMock.Verify(x => x.RentalsRepository.UpdateAsync(It.Is<RentalEntity>(y =>
                y.Id == request.RentalId && y.Units == request.Units && y.PreparationTimeInDays == request.PreparationTimeInDays)), Times.Once);
            _unitOfWorkMock.Verify(x => x.BookingsRepository.BulkUpdateAsync(It.Is<IEnumerable<BookingEntity>>(y => !y.Any())), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_WhenThereAreNoBookingOverBooking_ThenUpdatesExistingBookingAndReturnsUpdatedRental()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                RentalId = 1,
                Units = 5,
                PreparationTimeInDays = 2
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 6
            };

            var updatedRentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 5
            };

            var availableBookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = rentalEntity.Id,
                    Id = 9,
                    UnitId = 1,
                    BookingNights = 5,
                    BookingStart =  DateTime.Parse("2022-06-01"),
                    BookingEnd = DateTime.Parse("2022-06-01").AddDays(5 - 1),
                    PreparationStart = DateTime.Parse("2022-06-01").AddDays(5),
                    PreparationEnd = DateTime.Parse("2022-06-01").AddDays(5 + rentalEntity.PreparationTimeInDays - 1)
                }
            };

            var expectedResult = new RentalViewModel
            {
                Id = request.RentalId,
                Units = request.Units,
                PreparationTimeInDays = request.PreparationTimeInDays
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository
                    .GetBookingsAsync(It.Is<int>(y => y == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .UpdateAsync(It.Is<RentalEntity>(y => y.Units == 5 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .ReturnsAsync(updatedRentalEntity);

            _mapperMock.Setup(x =>
                    x.Map<RentalEntity>(It.Is<PutRentalRequest>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2)))
                .Returns(updatedRentalEntity);

            _mapperMock.Setup(x =>
                    x.Map<RentalViewModel>(It.Is<RentalEntity>(y =>
                        y.Units == 5 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .Returns(expectedResult);

            // Act
            var result = await _rentalsService.UpdateAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Success, result.Status);
            Assert.AreEqual(expectedResult, result.Result);

            _unitOfWorkMock.Verify(x => x.RentalsRepository.UpdateAsync(It.Is<RentalEntity>(y =>
                y.Id == request.RentalId && y.Units == request.Units && y.PreparationTimeInDays == request.PreparationTimeInDays)), Times.Once);
            _unitOfWorkMock.Verify(x => x.BookingsRepository.BulkUpdateAsync(It.Is<IEnumerable<BookingEntity>>(y => y.Count() == 1)), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_WhenThereIsOverBookingDueToDecreasingUnits_ThenReturnsConflictResponse()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                RentalId = 1,
                Units = 4,
                PreparationTimeInDays = 2
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 5
            };

            var availableBookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = request.RentalId,
                    Id = 9,
                    UnitId = 5
                }
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .UpdateAsync(It.Is<RentalEntity>(y => y.Units == 4 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository
                    .GetBookingsAsync(It.Is<int>(y => y == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            _mapperMock.Setup(x =>
                    x.Map<RentalEntity>(It.Is<PutRentalRequest>(y =>
                        y.Units == 4 && y.PreparationTimeInDays == 2)))
                .Returns(rentalEntity);

            // Act
            var result = await _rentalsService.UpdateAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Conflict, result.Status);
        }

        [Test]
        public async Task UpdateAsync_WhenThereIsOverBookingDueToIncreasingPreparationTime_ThenReturnsConflictResponse()
        {
            // Arrange
            var request = new PutRentalRequest
            {
                RentalId = 1,
                Units = 1,
                PreparationTimeInDays = 3
            };

            var rentalEntity = new RentalEntity
            {
                Id = 1,
                PreparationTimeInDays = 2,
                Units = 1
            };

            var availableBookings = new List<BookingEntity>
            {
                new BookingEntity
                {
                    RentalId = request.RentalId,
                    Id = 9,
                    UnitId = 1,
                    BookingStart = DateTime.Parse("2022-01-01"),
                    BookingNights = 10,
                    BookingEnd = DateTime.Parse("2022-01-10"),
                    PreparationStart = DateTime.Parse("2022-01-11"),
                    PreparationEnd = DateTime.Parse("2022-01-12")
                },
                new BookingEntity
                {
                    RentalId = request.RentalId,
                    Id = 10,
                    UnitId = 1,
                    BookingStart = DateTime.Parse("2022-01-13"),
                    BookingNights = 2,
                    BookingEnd = DateTime.Parse("2022-01-14"),
                    PreparationStart = DateTime.Parse("2022-01-15"),
                    PreparationEnd = DateTime.Parse("2022-01-16")
                },
            };

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .GetByIdAsync(It.Is<int>(y => y == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.RentalsRepository
                    .UpdateAsync(It.Is<RentalEntity>(y => y.Units == 1 && y.PreparationTimeInDays == 2 && y.Id == 1)))
                .ReturnsAsync(rentalEntity);

            _unitOfWorkMock
                .Setup(x => x.BookingsRepository
                    .GetBookingsAsync(It.Is<int>(y => y == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(availableBookings);

            _mapperMock.Setup(x =>
                    x.Map<RentalEntity>(It.Is<PutRentalRequest>(y =>
                        y.Units == 1 && y.PreparationTimeInDays == 2)))
                .Returns(rentalEntity);

            // Act
            var result = await _rentalsService.UpdateAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServiceResponse<RentalViewModel>>(result);
            Assert.AreEqual(ResponseStatus.Conflict, result.Status);
        }

        #endregion

        #endregion
    }
}
