using AutoMapper;
using System.Collections.Generic;
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
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly int _firstUnitId = 1;

        #endregion

        #region Constructor

        public BookingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse<BookingViewModel>> GetAsync(GetBookingRequest request)
        {
            var result = await _unitOfWork.BookingsRepository.GetByIdAsync(request.BookingId);

            if (result == null)
            {
                return new ServiceResponse<BookingViewModel>
                {
                    Status = ResponseStatus.BookingNotFound
                };
            }
            
            return new ServiceResponse<BookingViewModel>
            {
                Result = _mapper.Map<BookingViewModel>(result),
                Status = ResponseStatus.Success
            };
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(BookingBindingModel request)
        {
            var rental = await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId);

            if (rental == null)
            {
                return new ServiceResponse<ResourceIdViewModel>
                {
                    Status = ResponseStatus.RentalNotFound
                };
            }

            var availableUnits = await GetAvailableUnitsAsync(request, rental);

            if (!availableUnits.Any())
            {
                return new ServiceResponse<ResourceIdViewModel>
                {
                    Status = ResponseStatus.Conflict
                };
            }

            var createdBooking = await SaveBookingAsync(request, rental, availableUnits.First());

            return new ServiceResponse<ResourceIdViewModel>
            {
                Result = _mapper.Map<ResourceIdViewModel>(createdBooking),
                Status = ResponseStatus.Success
            };
        }

        #endregion

        #region Private methods

        private async Task<IEnumerable<int>> GetAvailableUnitsAsync(BookingBindingModel request, RentalEntity rental)
        {
            var overlappedBookings = await _unitOfWork.BookingsRepository.GetBookingsAsync(
                request.RentalId,
                request.Start,
                request.Start.AddDays(request.Nights + rental.PreparationTimeInDays - 1));

            var unitsList = Enumerable.Range(_firstUnitId, rental.Units);
            var bookedUnits = overlappedBookings.Select(x => x.UnitId);

            return unitsList.Except(bookedUnits);
        }

        private async Task<BookingEntity> SaveBookingAsync(BookingBindingModel request, RentalEntity rental, int unit)
        {
            var bookingToCreate = _mapper.Map<BookingEntityCreate>(request);
            bookingToCreate.UnitId = unit;
            bookingToCreate.PreparationTime = rental.PreparationTimeInDays;

            var createdBooking = await _unitOfWork.BookingsRepository.AddAsync(bookingToCreate);
            await _unitOfWork.CommitAsync();
            return createdBooking;
        }

        #endregion
    }
}
