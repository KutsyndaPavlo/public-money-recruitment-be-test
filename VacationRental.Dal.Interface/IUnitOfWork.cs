using System;

namespace VacationRental.Dal.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingsRepository BookingsRepository { get; }

        IRentalsRepository RentalsRepository { get; }

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
