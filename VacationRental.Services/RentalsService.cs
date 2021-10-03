using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Services
{
    public class RentalsService : IRentalsService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public RentalsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        public async Task<ServiceResponse<RentalViewModel>> GetByIdAsync(GetRentalRequest request)
        {
            var result = await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId);

            if (result == null)
            {
                return new ServiceResponse<RentalViewModel>
                {
                    Status = ResponseStatus.RentalNotFound
                };
            }

            return new ServiceResponse<RentalViewModel>
            {
                Status = ResponseStatus.Success,
                Result = _mapper.Map<RentalViewModel>(
                    await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId))
            };
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(RentalBindingModel request)
        {
            var createdRental = await _unitOfWork.RentalsRepository.AddAsync(_mapper.Map<RentalEntityCreate>(request));

            return new ServiceResponse<ResourceIdViewModel>
            {
                Result = _mapper.Map<ResourceIdViewModel>(createdRental),
                Status = ResponseStatus.Success
            };
            ;
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> UpdateAsync(PutRentalRequest request)
        {
            var rental = await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId);

            if (rental == null)
            {

                return new ServiceResponse<ResourceIdViewModel>
                {
                    Status = ResponseStatus.RentalNotFound
                };
            }

            if (!RentalChanged(request, rental))
            {
                return new ServiceResponse<ResourceIdViewModel>
                {
                    Result = _mapper.Map<ResourceIdViewModel>(rental),
                    Status = ResponseStatus.Success
                };
            }

            var bookings = await _unitOfWork.BookingsRepository.GetBookingsAsync(
                rental.Id,
                DateTime.Now.Date,
                DateTime.MaxValue.Date);


            if (IsOverlappingDueToUnitDecreasing(bookings, rental, request.Units) ||
                IsOverlappingDueToPreparationTimeIncreasing(bookings, rental, request.PreparationTimeInDays))

            {
                return new ServiceResponse<ResourceIdViewModel>
                {
                    Status = ResponseStatus.Conflict
                };
            }

            var updatedRental = await UpdateRentalAndBookings(request, bookings);

            return new ServiceResponse<ResourceIdViewModel>
            {
                Result = _mapper.Map<ResourceIdViewModel>(updatedRental),
                Status = ResponseStatus.Success
            };
        }

        #endregion

        #region Private methods

        private bool RentalChanged(PutRentalRequest request, RentalEntity rental)
        {
            return request.PreparationTimeInDays != rental.PreparationTimeInDays
                   || request.Units != rental.Units;
        }

        private bool IsOverlappingDueToUnitDecreasing(IEnumerable<BookingEntity> bookings, RentalEntity rental, int updatedUnits)
        {
            if (updatedUnits >= rental.Units)
            {
                return false;
            }

            var firstDecreasedUnitId = updatedUnits + 1;
            var decreasedUnitsCount = rental.Units - updatedUnits;
            var decreasedUnits = Enumerable.Range(firstDecreasedUnitId, decreasedUnitsCount).ToList();

            return bookings.Any(booking => decreasedUnits.Any(unitId => unitId == booking.UnitId));
        }

        private bool IsOverlappingDueToPreparationTimeIncreasing(IEnumerable<BookingEntity> bookings,
                                                                 RentalEntity rental,
                                                                 int updatedPreparationTimeInDays)
        {
            if (updatedPreparationTimeInDays <= rental.PreparationTimeInDays)
            {
                return false;
            }

            return bookings.GroupBy(x => x.UnitId)
                           .Any(bookingsGroup =>
                           {
                               var isOverlapping = false;
                               BookingEntity previous = null;
                               foreach (var booking in bookingsGroup.OrderBy(x => x.BookingStart))
                               {
                                   if (previous != null)
                                   {
                                       var daysForNextBooking = (booking.BookingStart - previous.BookingEnd).Days;
                                       if (daysForNextBooking <= updatedPreparationTimeInDays)
                                       {
                                           isOverlapping = true;
                                           break;
                                       }
                                   }

                                   previous = booking;
                               }

                               return isOverlapping;
                           });
        }

        private async Task<RentalEntity> UpdateRentalAndBookings(PutRentalRequest request, IEnumerable<BookingEntity> bookings)
        {
            _unitOfWork.BeginTransaction();

            var updatedRental = await UpdateRental(request);
            var updatedBookings = await UpdateBookings(request, bookings);

            if (updatedRental == null || updatedBookings == null)
            { 
                _unitOfWork.RollbackTransaction();
            }

            _unitOfWork.CommitTransaction();

            return updatedRental;

            async Task<RentalEntity> UpdateRental(PutRentalRequest putRequest)
            {
                var rentalEntity = _mapper.Map<RentalEntity>(putRequest);
                rentalEntity.Id = request.RentalId;
                return await _unitOfWork.RentalsRepository.UpdateAsync(rentalEntity);
            }

            async Task<IEnumerable<BookingEntity>> UpdateBookings(PutRentalRequest putRequest, IEnumerable<BookingEntity> currentBookings)
            {
                foreach (var booking in currentBookings)
                {
                    booking.PreparationEnd = booking.PreparationStart.AddDays(putRequest.PreparationTimeInDays - 1);
                }

                return await _unitOfWork.BookingsRepository.BulkUpdateAsync(currentBookings);
            }
        }

        #endregion
    }
}
