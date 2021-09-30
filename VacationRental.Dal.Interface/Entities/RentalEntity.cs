using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Dal.Interface.Entities
{
    public class RentalEntity
    {
        public int Id { get; set; }

        public int Units { get; set; }

        public int PreparationTimeInDays { get; set; }
    }
}
