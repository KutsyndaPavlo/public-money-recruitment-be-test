using System;
using System.Collections.Generic;
using System.Text;

namespace VacationRental.Services.Models
{
    public class ServiceResponse<T> where T : class
    {
        public ResponseStatus Status { get; set; }

        public T Result { get; set; }
    }
}
