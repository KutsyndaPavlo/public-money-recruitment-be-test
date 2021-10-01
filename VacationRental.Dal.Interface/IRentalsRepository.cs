using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IRentalsRepository
    {
        Task<RentalEntity> GetByIdAsync(int id);

        Task<RentalEntity> AddAsync(RentalEntityCreate rentalToCreate);

        Task<RentalEntity> UpdateAsync(RentalEntity rentalToUpdate);
    }
}