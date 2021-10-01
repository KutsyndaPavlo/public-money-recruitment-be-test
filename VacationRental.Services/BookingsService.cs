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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<BookingViewModel>> GetAsync(GetBookingRequest request)
        {
            return new ServiceResponse<BookingViewModel> { Result = _mapper.Map<BookingViewModel>(await _unitOfWork.BookingsRepository.GetByIdAsync(request.BookingId)), Status = ResponseStatus.Success };
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(BookingBindingModel rentalEntityCreate) 
        {
            var rentals = await _unitOfWork.RentalsRepository.GetByIdAsync(rentalEntityCreate.RentalId);


            var bookings = await _unitOfWork.BookingsRepository.GetBookingsAsync(rentalEntityCreate.RentalId,
                                            rentalEntityCreate.Start,
                                            rentalEntityCreate.Start.AddDays(rentalEntityCreate.Nights + rentals.PreparationTimeInDays - 1));



            if (bookings.Count() < rentals.Units)
            {

                var unitId = Enumerable.Range(1, rentals.Units).Except(bookings.Select(x => x.UnitId)).First();

                var mapu = _mapper.Map<BookingEntityCreate>(rentalEntityCreate);
                mapu.UnitId = unitId;
                mapu.PreparationTime = rentals.PreparationTimeInDays;

                var ee = await _unitOfWork.BookingsRepository.AddAsync(mapu);
                await _unitOfWork.CommitAsync();
                return new ServiceResponse<ResourceIdViewModel> { Result = _mapper.Map<ResourceIdViewModel>(ee), Status = ResponseStatus.Success };
            }

            return null;
        }

    }
}
