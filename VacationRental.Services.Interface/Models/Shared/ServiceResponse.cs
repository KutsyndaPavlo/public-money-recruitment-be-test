using VacationRental.Services.Interface.Enums;

namespace VacationRental.Services.Interface.Models.Shared
{
    public class ServiceResponse<T> where T : class
    {
        public ResponseStatus Status { get; set; }

        public T Result { get; set; }
    }
}
