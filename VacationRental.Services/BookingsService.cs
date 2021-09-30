using AutoMapper;
using System.Linq;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IRentalsRepository _rentalsRepository;
        private readonly IMapper _mapper;

        public BookingsService(IBookingsRepository bookingsRepository, IRentalsRepository rentalsRepository, IMapper mapper)
        {
            _bookingsRepository = bookingsRepository;
            _rentalsRepository = rentalsRepository;
            _mapper = mapper;
        }
        public ServiceResponse<BookingViewModel> Get(GetBookingRequest request)
        {
            return new ServiceResponse<BookingViewModel> { Result = _mapper.Map<BookingViewModel>(_bookingsRepository.GetById(request.BookingId)), Status = ResponseStatus.Success };
        }

        public ServiceResponse<ResourceIdViewModel> Add(BookingBindingModel rentalEntityCreate) 
        {
            var rentals = _rentalsRepository.GetById(rentalEntityCreate.RentalId);


            var bookings = _bookingsRepository.GetBookings(rentalEntityCreate.RentalId,
                                            rentalEntityCreate.Start,
                                            rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights + rentals.PreparationTimeInDays - 1)).ToList();



            if (bookings.Count() < rentals.Units)
            {

                var unitId = Enumerable.Range(1, rentals.Units).Except(bookings.Select(x => x.UnitId)).First();

                var mapu = _mapper.Map<BookingEntityCreate>(rentalEntityCreate);
                mapu.UnitId = unitId;
                mapu.PreparationTime = rentals.PreparationTimeInDays;
                return new ServiceResponse<ResourceIdViewModel> { Result = _mapper.Map<ResourceIdViewModel>(_bookingsRepository.Add(mapu)), Status = ResponseStatus.Success };
            }

            return null;
        }

    }
}
