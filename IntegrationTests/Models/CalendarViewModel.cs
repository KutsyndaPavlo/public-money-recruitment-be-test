using System.Collections.Generic;

namespace IntegrationTests.Models
{
    public class CalendarViewModel
    {
        public int RentalId { get; set; }

        public List<CalendarDateViewModel> Dates { get; set; }
    }
}
