namespace VacationRental.Services.Interface.Models
{
    public class ServiceResponse<T> where T : class
    {
        public ResponseStatus Status { get; set; }

        public T Result { get; set; }
    }
}
