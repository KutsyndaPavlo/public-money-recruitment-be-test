using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Data.Entities
{
    public class BookingEntity
    {
        public int Id { get; set; }
        public int RentalId { get; set; }        
        public int UnitId { get; set; }
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }
        public int BookingNights { get; set; }

        public DateTime PreparationStart { get; set; }
        public DateTime PreparationEnd { get; set; }
    }
}
