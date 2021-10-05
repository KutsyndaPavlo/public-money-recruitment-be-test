using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VacationRental.Dal.InMemory.Repositories;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Tests.Repositories
{
    public class RentalsRepositoryTests
    {
        #region Fields

        private IRentalsRepository _rentalsRepository;
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

            _rentalsRepository = new RentalsRepository(_context);
        }

        #endregion

        #region Tests

        [Test]
        public async Task GetByIdAsync_WhenRentalExists_ThenReturnsRental()
        {
            // Arrange
            var existingEntity = await _context.Rentals.AddAsync(new RentalEntity { PreparationTimeInDays = 5, Units = 2 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _rentalsRepository.GetByIdAsync(existingEntity.Entity.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RentalEntity>(result);
            Assert.AreEqual(2, result.Units);
            Assert.AreEqual(5, result.PreparationTimeInDays);
        }

        [Test]
        public async Task AddAsync_WhenRentalParametersAreValid_ThenReturnsCreatedRental()
        {
            // Arrange
            var rentalEntityCreate = new RentalEntityCreate { PreparationTimeInDays = 5, Units = 2 };

            // Act
            var result = await _rentalsRepository.AddAsync(rentalEntityCreate);
            await _context.SaveChangesAsync();

            // Assert
            Assert.IsNotNull(result);
            var createdRentalEntity = await _rentalsRepository.GetByIdAsync(result.Id);
            Assert.IsNotNull(createdRentalEntity);
            Assert.IsInstanceOf<RentalEntity>(result);
            Assert.AreEqual(2, result.Units);
            Assert.AreEqual(5, result.PreparationTimeInDays);
        }

        [Test]
        public async Task UpdateAsync_WhenRentalParametersAreValid_ThenReturnsUpdatedRental()
        {
            // Arrange
            var existingEntity = await _context.Rentals.AddAsync(new RentalEntity { PreparationTimeInDays = 5, Units = 2 });
            await _context.SaveChangesAsync();
            var rentalEntityToUpdate = new RentalEntity
            {
                Id = existingEntity.Entity.Id,
                PreparationTimeInDays = 10, 
                Units = 15
            };

            // Act
            var result = await _rentalsRepository.UpdateAsync(rentalEntityToUpdate);
            await _context.SaveChangesAsync();

            // Assert
            Assert.IsNotNull(result);

            var updatedRentalEntity = await _rentalsRepository.GetByIdAsync(result.Id);
            Assert.IsNotNull(updatedRentalEntity);
            Assert.IsInstanceOf<RentalEntity>(result);
            Assert.AreEqual(15, result.Units);
            Assert.AreEqual(10, result.PreparationTimeInDays);
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
