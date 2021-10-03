using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IBookingsRepository : IBaseRepository<BookingEntity, BookingEntityCreate>
    {
        Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<BookingEntity>> BulkUpdateAsync(IEnumerable<BookingEntity> bookingsToUpdate);
    }
}