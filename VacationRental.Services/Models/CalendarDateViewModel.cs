using System;
using System.Collections.Generic;
using VacationRental.Services.Models;

namespace VacationRental.Api.Models
{
    public class CalendarDateViewModel
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }

        public List<CalendarPreparationTimeViewModel> PreparationTimes { get; set; }
    }
}
