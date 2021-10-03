using System;

namespace VacationRental.Services.Interface.Models.Calendar
{
    public class GetCalendarRequest
    {
        public int RentalId { get; set; }

        public DateTime StartDate { get; set; }

        public int Nights { get; set; }
    }
}
