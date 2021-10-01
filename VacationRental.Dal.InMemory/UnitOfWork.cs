using System;
using System.Threading.Tasks;
using VacationRental.Dal.InMemory.Repositories;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.InMemory
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VacationRentalDbContext _dbContext;

        private bool _disposed = false;

        public UnitOfWork(VacationRentalDbContext dbContext)
        {
            _dbContext = dbContext;
            BookingsRepository = new BookingsRepository(dbContext);
            RentalsRepository = new RentalsRepository(dbContext);
        }

        public IBookingsRepository BookingsRepository { get; }

        public IRentalsRepository RentalsRepository { get; }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
