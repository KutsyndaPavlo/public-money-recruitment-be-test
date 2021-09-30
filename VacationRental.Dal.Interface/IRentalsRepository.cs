using VacationRental.Dal.Interface.Entities;

namespace VacationRental.Dal.Interface
{
    public interface IRentalsRepository
    {
        RentalEntity Add(RentalEntityCreate rentalEntityCreate);
        RentalEntity GetById(int id);
        RentalEntity Update(RentalEntity rentalEntity);
    }
}