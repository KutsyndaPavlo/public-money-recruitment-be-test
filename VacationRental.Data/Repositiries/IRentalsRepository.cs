using VacationRental.Data.Entities;

namespace VacationRental.Data.Repositiries
{
    public interface IRentalsRepository
    {
        RentalEntity Add(RentalEntityCreate rentalEntityCreate);
        RentalEntity GetById(int id);
        RentalEntity Update(RentalEntity rentalEntity);
    }
}