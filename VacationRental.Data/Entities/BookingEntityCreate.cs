using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Data.Entities
{
    public class BookingEntityCreate
    {
        public int RentalId { get; set; }

        public int UnitId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }

        public int PreparationTime { get; set; }
    }
}
