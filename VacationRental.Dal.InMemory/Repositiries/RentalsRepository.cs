using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Repositories
{
    public class RentalsRepository : IRentalsRepository
    {
        #region Fieelds

        private readonly VacationRentalDbContext _dbContext;
        //private int _counter = 0;

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
           return await _dbContext.Rentals.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RentalEntity> AddAsync(RentalEntityCreate rentalEntityCreate)
        {
            var rental = new RentalEntity
            {
                //Id = Interlocked.Increment(ref _counter),
                Units = rentalEntityCreate.Units,
                PreparationTimeInDays = rentalEntityCreate.PreparationTimeInDays
            };

            await _dbContext.Rentals.AddAsync(rental);
            await _dbContext.SaveChangesAsync();

            return rental;
        }

        public async Task<RentalEntity> UpdateAsync(RentalEntity rentalEntity)
        {
            var rental = await _dbContext.Rentals.FirstOrDefaultAsync(x => x.Id == rentalEntity.Id);

            rental.Units = rentalEntity.Units;
            rental.PreparationTimeInDays = rentalEntity.PreparationTimeInDays;
            await _dbContext.SaveChangesAsync();

            return rental;
        }

        #endregion
    }
}
