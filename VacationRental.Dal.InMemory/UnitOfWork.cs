using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task RejectChangesAsync()
        {
            foreach (var entry in _dbContext
                .ChangeTracker
                .Entries()
                .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
