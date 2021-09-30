using System.Collections.Generic;

namespace VacationRental.Services.Interface.Models
{
    public class CalendarViewModel
    {
        public int RentalId { get; set; }

        public List<CalendarDateViewModel> Dates { get; set; }
    }
}
