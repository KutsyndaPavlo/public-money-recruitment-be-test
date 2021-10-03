using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using VacationRental.Dal.InMemory.Repositories;
using VacationRental.Dal.Interface;

namespace VacationRental.Dal.InMemory
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly VacationRentalDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        #endregion

        #region Constructor

        public UnitOfWork(VacationRentalDbContext dbContext)
        {
            _dbContext = dbContext;
            BookingsRepository = new BookingsRepository(dbContext);
            RentalsRepository = new RentalsRepository(dbContext);
        }

        #endregion


        #region Properties

        public IBookingsRepository BookingsRepository { get; }

        public IRentalsRepository RentalsRepository { get; }

        #endregion

        #region Methods

        public void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = _dbContext.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                    }
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RollbackTransaction()
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

            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
        }

        #endregion
    }
}
