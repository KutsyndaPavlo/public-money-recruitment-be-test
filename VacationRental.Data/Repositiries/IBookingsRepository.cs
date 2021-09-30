using System;
using System.Collections.Generic;
using VacationRental.Data.Entities;

namespace VacationRental.Data.Repositiries
{
    public interface IBookingsRepository
    {
        BookingEntity GetById(int id);

        BookingEntity Add(BookingEntityCreate rentalEntityCreate);
        IEnumerable<BookingEntity> GetBookings(int rentalId, DateTime? start = null, DateTime? end = null);

        BookingEntity Update(BookingEntity rentalEntityCreate);


    }
}