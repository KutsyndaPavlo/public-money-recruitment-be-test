namespace VacationRental.Dal.Interface.Entities
{
    public class RentalEntityCreate : BaseEntity
    {
        public int Units { get; set; }

        public int PreparationTimeInDays { get; set; }
    }
}
