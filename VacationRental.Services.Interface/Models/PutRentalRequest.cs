namespace VacationRental.Services.Interface.Models
{
    public class PutRentalRequest
    {
        public int RentalId { get; set; }
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}
