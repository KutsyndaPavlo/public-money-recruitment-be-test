using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ServiceResponse<BookingViewModel>> GetAsync(GetBookingRequest request)
        {
            return new ServiceResponse<BookingViewModel> { Result = _mapper.Map<BookingViewModel>(await _bookingsRepository.GetByIdAsync(request.BookingId)), Status = ResponseStatus.Success };
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(BookingBindingModel rentalEntityCreate) 
        {
            var rentals = await _rentalsRepository.GetByIdAsync(rentalEntityCreate.RentalId);


            var bookings = await _bookingsRepository.GetBookingsAsync(rentalEntityCreate.RentalId,
                                            rentalEntityCreate.Start,
                                            rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights + rentals.PreparationTimeInDays - 1));



            if (bookings.Count() < rentals.Units)
            {

                var unitId = Enumerable.Range(1, rentals.Units).Except(bookings.Select(x => x.UnitId)).First();

                var mapu = _mapper.Map<BookingEntityCreate>(rentalEntityCreate);
                mapu.UnitId = unitId;
                mapu.PreparationTime = rentals.PreparationTimeInDays;
                return new ServiceResponse<ResourceIdViewModel> { Result = _mapper.Map<ResourceIdViewModel>(await _bookingsRepository.AddAsync(mapu)), Status = ResponseStatus.Success };
            }

            return null;
        }

    }
}
