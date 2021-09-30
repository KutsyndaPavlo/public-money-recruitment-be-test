using System;

namespace VacationRental.Dal.Interface.Entities
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
