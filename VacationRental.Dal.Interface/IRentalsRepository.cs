using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IRentalsRepository : IBaseRepository<RentalEntity, RentalEntityCreate>
    {
        Task<RentalEntity> UpdateAsync(RentalEntity rentalToUpdate);
    }
}