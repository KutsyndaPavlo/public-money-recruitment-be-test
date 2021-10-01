using System.Threading.Tasks;
using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IRentalsRepository
    {
        Task<RentalEntity> GetByIdAsync(int id);

        Task<RentalEntity> AddAsync(RentalEntityCreate rentalEntityCreate);

        Task<RentalEntity> UpdateAsync(RentalEntity rentalEntity);
    }
}