using System;
using System.Threading.Tasks;

namespace VacationRental.Dal.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingsRepository BookingsRepository { get; }

        IRentalsRepository RentalsRepository { get; }

        Task<int> CommitAsync();

        Task RejectChangesAsync();
    }
}
