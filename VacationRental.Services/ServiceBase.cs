using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services
{
    public class ServiceBase
    {
        protected static ServiceResponse<T> GetServiceResponse<T>(ResponseStatus status, T result = null) where T : class
        {
            return new ServiceResponse<T>
            {
                Status = status,
                Result = result
            };
        }
    }
}
