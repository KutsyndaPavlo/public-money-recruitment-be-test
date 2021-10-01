using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Repositories
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly VacationRentalDbContext _dbContext;

        #region Constructor

        public BookingsRepository(VacationRentalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        public async Task<BookingEntity> GetByIdAsync(int id)
        {
            return await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BookingEntity> AddAsync(BookingEntityCreate rentalEntityCreate)
        {
            var booking = new BookingEntity
            {
                //Id = Interlocked.Increment(ref counter),
                BookingNights = rentalEntityCreate.Nights,
                RentalId = rentalEntityCreate.RentalId,
                UnitId = rentalEntityCreate.UnitId,
                BookingStart = rentalEntityCreate.Start,
                BookingEnd = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights - 1),
                PreparationStart = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights),
                PreparationEnd = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights).AddDays(rentalEntityCreate.PreparationTime - 1)
            };

            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }

        public async Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime? start = null, DateTime? end = null)
        {
            if (start.HasValue && end.HasValue)
            {
                return _dbContext.Bookings.Where(x => x.RentalId == rentalId && x.PreparationEnd >= start && x.BookingStart <= end);
            }


            return _dbContext.Bookings.Where(x => x.RentalId == rentalId && x.PreparationEnd >= end && x.PreparationStart >= end);

        }

        public async Task<BookingEntity> UpdateAsync(BookingEntity rentalEntityCreate)
        {
            var resu = await _dbContext.Bookings.FirstOrDefaultAsync(x=>x.Id == rentalEntityCreate.Id);
            resu.PreparationEnd = rentalEntityCreate.PreparationEnd;
            await _dbContext.SaveChangesAsync();
            return resu;
        }
    }
}
