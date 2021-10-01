using System.Threading.Tasks;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services.Interface.Validation
{
    public interface ICalendarValidationService
    {
        Task<ServiceResponse<string>> ValidateGetRequestAsync(GetCalendarRequest request);
    }
}
