using System.Collections.Generic;

namespace VacationRental.Services.Interface.Models.Calendar
{
    public class CalendarViewModel
    {
        public int RentalId { get; set; }

        public List<CalendarDateViewModel> Dates { get; set; }
    }
}
