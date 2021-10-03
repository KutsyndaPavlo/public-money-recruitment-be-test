using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Repositories
{
    public class RentalsRepository : IRentalsRepository
    {
        #region Fieelds

        private readonly VacationRentalDbContext _dbContext;

        #endregion

        #region Constructor

        public RentalsRepository(VacationRentalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public async Task<RentalEntity> GetByIdAsync(int id)
        {
           return await _dbContext.Rentals.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RentalEntity> AddAsync(RentalEntityCreate rentalToCreate)
        {
            var rental = new RentalEntity
            {
                Units = rentalToCreate.Units,
                PreparationTimeInDays = rentalToCreate.PreparationTimeInDays
            };

            await _dbContext.Rentals.AddAsync(rental);
            await _dbContext.SaveChangesAsync();

            return rental;
        }

        public async Task<RentalEntity> UpdateAsync(RentalEntity rentalToUpdate)
        {
            var currentRental = await _dbContext.Rentals.FirstOrDefaultAsync(x => x.Id == rentalToUpdate.Id);

            if (currentRental == null)
            {
                throw new Exception();   //ToDO
            }

            currentRental.Units = rentalToUpdate.Units;
            currentRental.PreparationTimeInDays = rentalToUpdate.PreparationTimeInDays;

            await _dbContext.SaveChangesAsync();

            return currentRental;
        }

        #endregion
    }
}
