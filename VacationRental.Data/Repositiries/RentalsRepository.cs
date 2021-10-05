using System.Collections.Generic;
using System.Threading;

namespace VacationRental.Dal.InMemory.Repositiries
{
    public class RentalsRepository : VacationRental.Dal.Interface.IRentalsRepository
    {
        #region Fieelds

        private int counter = 0;
        private readonly IDictionary<int, RentalEntity> _rentals = new Dictionary<int, RentalEntity>();

        #endregion

        #region Methods

        public RentalEntity GetById(int id)
        {
            if (!_rentals.ContainsKey(id))
            {
                return null;
            }

            return _rentals[id];
        }

        public RentalEntity Add(RentalEntityCreate rentalEntityCreate)
        {
            var rental = new RentalEntity
            {
                Id = Interlocked.Increment(ref counter),
                Units = rentalEntityCreate.Units,
                PreparationTimeInDays = rentalEntityCreate.PreparationTimeInDays
            };

            _rentals.Add(rental.Id, rental);

            return rental;
        }

        public RentalEntity Update(RentalEntity rentalEntity)
        {
            var item = _rentals[rentalEntity.Id];

            item.Units = rentalEntity.Units;
            item.PreparationTimeInDays = rentalEntity.PreparationTimeInDays;

            return item;
        }


        #endregion
    }
}
