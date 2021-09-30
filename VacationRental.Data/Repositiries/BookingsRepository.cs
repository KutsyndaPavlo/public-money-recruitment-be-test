using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VacationRental.Data.Entities;

namespace VacationRental.Data.Repositiries
{
    public class BookingsRepository : IBookingsRepository
    {
        private int counter = 0;
        private readonly IDictionary<int, BookingEntity> _bookings = new Dictionary<int, BookingEntity>();

        public BookingEntity GetById(int id)
        {
            if (!_bookings.ContainsKey(id))
            {
                return null;
            }

            return _bookings[id];
        }

        public BookingEntity Add(BookingEntityCreate rentalEntityCreate)
        {
            var booking = new BookingEntity
            {
                Id = Interlocked.Increment(ref counter),
                BookingNights = rentalEntityCreate.Nights,
                RentalId = rentalEntityCreate.RentalId,
                UnitId =rentalEntityCreate.UnitId,
                BookingStart = rentalEntityCreate.Start, 
                BookingEnd = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights -1),
                PreparationStart = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights),
                PreparationEnd = rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights).AddDays(rentalEntityCreate.PreparationTime-1)
            };

            _bookings.Add(booking.Id, booking);

            return booking;
        }

        public IEnumerable<BookingEntity> GetBookings(int rentalId, DateTime? start = null, DateTime? end = null)
        {
            if (start.HasValue && end.HasValue)
            {
                return _bookings.Values.Where(x => x.RentalId == rentalId && x.PreparationEnd >= start && x.BookingStart <= end);
            }


           return  _bookings.Values.Where(x => x.RentalId == rentalId && x.PreparationEnd >= end && x.PreparationStart >= end);

        }

        public BookingEntity Update(BookingEntity rentalEntityCreate)
        {
            _bookings[rentalEntityCreate.Id].PreparationEnd = rentalEntityCreate.PreparationEnd;

            return _bookings[rentalEntityCreate.Id];
        }
    }
}
