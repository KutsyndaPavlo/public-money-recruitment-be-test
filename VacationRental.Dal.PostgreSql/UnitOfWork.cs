using Npgsql;
using System;
using VacationRental.Dal.Interface;
using VacationRental.Dal.PostgreSql.Repositories;

namespace VacationRental.Dal.PostgreSql
{
    public class UnitOfWork : IUnitOfWork
    {

        #region Fields

        private bool _disposed = false;
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        #endregion

        #region Constructor

        public UnitOfWork(NpgsqlConnection connection)
        {
            _connection = connection;
            BookingsRepository = new BookingsRepository(_connection);
            RentalsRepository = new RentalsRepository(_connection);
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
                _transaction = _connection.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }

        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        _connection.Close();
                    }

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

        #endregion
    }
}
