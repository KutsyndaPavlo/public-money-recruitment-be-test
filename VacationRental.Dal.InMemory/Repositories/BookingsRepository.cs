using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.InMemory.Repositories
{
    public class BookingsRepository : IBookingsRepository
    {
        #region Fields

        private readonly VacationRentalDbContext _dbContext;

        #endregion

        #region Constructor

        public BookingsRepository(VacationRentalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public async Task<BookingEntity> GetByIdAsync(int id)
        {
            return await _dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<BookingEntity>> GetBookingsAsync(int rentalId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Bookings.Where(x => x.RentalId == rentalId && x.PreparationEnd >= startDate && x.BookingStart <= endDate)
                                            .AsNoTracking()
                                            .ToListAsync();
        }

        public async Task<BookingEntity> AddAsync(BookingEntityCreate bookingToCreate)
        {
            var booking = new BookingEntity
            {
                BookingNights = bookingToCreate.Nights,
                RentalId = bookingToCreate.RentalId,
                UnitId = bookingToCreate.UnitId,
                BookingStart = bookingToCreate.Start,
                BookingEnd = bookingToCreate.Start.AddDays(bookingToCreate.Nights - 1),
                PreparationStart = bookingToCreate.Start.AddDays(bookingToCreate.Nights),
                PreparationEnd = bookingToCreate.Start.AddDays(bookingToCreate.Nights).AddDays(bookingToCreate.PreparationTime - 1)
            };

            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }

        public async Task<IEnumerable<BookingEntity>> BulkUpdateAsync(IEnumerable<BookingEntity> bookingsToUpdate)
        {
            foreach (var booking in bookingsToUpdate)
            {
                var currentBooking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == booking.Id);

                if (currentBooking == null)
                {
                    throw new Exception();   //ToDO
                }
                currentBooking.PreparationEnd = booking.PreparationEnd;
            }

            await _dbContext.SaveChangesAsync();

            return bookingsToUpdate;
        }

        #endregion
    }
}
