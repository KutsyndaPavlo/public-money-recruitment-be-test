using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VacationRental.Dal.InMemory.Repositories;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Tests.Repositories
{
    public class BookingsRepositoryTests
    {
        #region Fields

        private IBookingsRepository _bookingsRepository;
        private VacationRentalDbContext _context;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VacationRentalDbContext>()
                .UseInMemoryDatabase(databaseName: "VacationRental1")
                .Options;
            _context = new VacationRentalDbContext(options);

            _bookingsRepository = new BookingsRepository(_context);
        }

        #endregion

        #region Tests

        [Test]
        public async Task GetByIdAsync_WhenBookingExists_ThenReturnsBooking()
        {
            // Arrange
            var existedBooking = await _context.Bookings.AddAsync(new BookingEntity { RentalId = 2 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingsRepository.GetByIdAsync(existedBooking.Entity.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BookingEntity>(result);
            Assert.AreEqual(2, result.RentalId);
        }

        [Test]
        public async Task AddAsync_WhenBookingParametersAreValid_ThenReturnsCreatedBooking()
        {
            // Arrange
            var bookingEntityCreate = new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-01")
            };

            // Act
            var result = await _bookingsRepository.AddAsync(bookingEntityCreate);
            await _context.SaveChangesAsync();

            // Assert
            Assert.IsNotNull(result);
            var createdBookingEntity = await _bookingsRepository.GetByIdAsync(result.Id);
            Assert.IsNotNull(createdBookingEntity);
            Assert.IsInstanceOf<BookingEntity>(result);
            Assert.AreEqual(1, result.RentalId);
            Assert.AreEqual(3, result.BookingNights);
            Assert.AreEqual(DateTime.Parse("2022-01-01"), result.BookingStart);
            Assert.AreEqual(DateTime.Parse("2022-01-03"), result.BookingEnd);
            Assert.AreEqual(DateTime.Parse("2022-01-04"), result.PreparationStart);
            Assert.AreEqual(DateTime.Parse("2022-01-05"), result.PreparationEnd);
        }

        [Test]
        public async Task BulkUpdateAsync_WhenBoookingParametersAreValid_ThenReturnsUpdatedBooking()
        {
            // Arrange
            var bookingEntityCreate = new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-01")
            };
            var result = await _bookingsRepository.AddAsync(bookingEntityCreate);
            await _context.SaveChangesAsync();

            var updates = new List<BookingEntity>
            {
                new BookingEntity
                {
                    Id = result.Id,
                    BookingNights = 3,
                    BookingStart = DateTime.Parse("2022-01-01"),
                    BookingEnd = DateTime.Parse("2022-01-03"),
                    PreparationStart = DateTime.Parse("2022-01-04"),
                    PreparationEnd = DateTime.Parse("2022-01-09"),
                }
            };

            // Act
            var updatesResult = await _bookingsRepository.BulkUpdateAsync(updates);
            await _context.SaveChangesAsync();

            // Assert
            Assert.IsNotNull(result);
            var createdBookingEntity = await _bookingsRepository.GetByIdAsync(result.Id);
            Assert.IsNotNull(createdBookingEntity);
            Assert.IsInstanceOf<BookingEntity>(result);
            Assert.AreEqual(1, result.RentalId);
            Assert.AreEqual(3, result.BookingNights);
            Assert.AreEqual(DateTime.Parse("2022-01-01"), updatesResult.FirstOrDefault(x => x.Id == result.Id).BookingStart);
            Assert.AreEqual(DateTime.Parse("2022-01-03"), updatesResult.FirstOrDefault(x => x.Id == result.Id).BookingEnd);
            Assert.AreEqual(DateTime.Parse("2022-01-04"), updatesResult.FirstOrDefault(x => x.Id == result.Id).PreparationStart);
            Assert.AreEqual(DateTime.Parse("2022-01-09"), updatesResult.FirstOrDefault(x => x.Id == result.Id).PreparationEnd);
        }

        [Test]
        public async Task GetBookingsAsync_WhenParametersAreValid_ThenReturnsCorrectBookings()
        {
            // Arrange
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 5,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-01")
            });
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-05")
            });
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-21")
            });

            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-09")
            });
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 2,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-11")
            });
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 10,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-11")
            });
            await _bookingsRepository.AddAsync(new BookingEntityCreate
            {
                RentalId = 1,
                Nights = 3,
                PreparationTime = 2,
                Start = DateTime.Parse("2022-01-16")
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingsRepository.GetBookingsAsync(1, DateTime.Parse("2022-01-10"), DateTime.Parse("2022-01-20"));

            // Assert
            Assert.IsNotNull(result); 
            Assert.IsInstanceOf<IEnumerable<BookingEntity>>(result); 
            Assert.AreEqual(4, result.Count());
        }

        #endregion

        #region TearDown

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #endregion
    }
}