using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IBookingsRepository
    {
        Task<BookingEntity> GetByIdAsync(int id);

        Task<BookingEntity> AddAsync(BookingEntityCreate bookingToCreate);

        Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<BookingEntity>> BulkUpdateAsync(IEnumerable<BookingEntity> bookingsToUpdate);
    }
}