using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IBookingsRepository
    {
        Task<BookingEntity> GetByIdAsync(int id);

        Task<BookingEntity> AddAsync(BookingEntityCreate rentalEntityCreate);

        Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime? start = null, DateTime? end = null);

        Task<BookingEntity> UpdateAsync(BookingEntity rentalEntityCreate);
    }
}