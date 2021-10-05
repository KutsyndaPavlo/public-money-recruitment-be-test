using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Dal.Interface;
using VacationRental.Dal.Interface.Entities;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Rentals;
using VacationRental.Services.Interface.Models.Shared;

namespace VacationRental.Services
{
    public class RentalsService : ServiceBase, IRentalsService
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

            return result == null 
                ? GetServiceResponse<RentalViewModel>(ResponseStatus.RentalNotFound) 
                : GetServiceResponse(ResponseStatus.Success, _mapper.Map<RentalViewModel>(result));
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(RentalBindingModel request)
        {
            var createdRental = await _unitOfWork.RentalsRepository.AddAsync(_mapper.Map<RentalEntityCreate>(request));

            return GetServiceResponse(ResponseStatus.Success, _mapper.Map<ResourceIdViewModel>(createdRental));
        }

        public async Task<ServiceResponse<RentalViewModel>> UpdateAsync(PutRentalRequest request)
        {
            var rental = await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId);

            if (rental == null)
            {
                return GetServiceResponse<RentalViewModel>(ResponseStatus.RentalNotFound);
            }

            if (!RentalChanged(request, rental))
            {
                return GetServiceResponse(ResponseStatus.Success, _mapper.Map<RentalViewModel>(rental));
            }

            var bookings = await _unitOfWork.BookingsRepository.GetBookingsAsync(
                rental.Id,
                DateTime.Now.Date,
                DateTime.MaxValue.Date);


            if (IsOverBookingDueToUnitDecreasing(bookings, rental, request.Units) ||
                IsOverBookingDueToPreparationTimeIncreasing(bookings, rental, request.PreparationTimeInDays))

            {
                return GetServiceResponse<RentalViewModel>(ResponseStatus.Conflict);
            }

            var updatedRental = await UpdateRentalAndBookings(request, bookings);

            return GetServiceResponse(ResponseStatus.Success, _mapper.Map<RentalViewModel>(updatedRental));
        }

        #endregion

        #region Private methods

        private bool RentalChanged(PutRentalRequest request, RentalEntity rental)
        {
            return request.PreparationTimeInDays != rental.PreparationTimeInDays
                   || request.Units != rental.Units;
        }

        private bool IsOverBookingDueToUnitDecreasing(IEnumerable<BookingEntity> bookings, RentalEntity rental, int updatedUnits)
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

        private bool IsOverBookingDueToPreparationTimeIncreasing(IEnumerable<BookingEntity> bookings,
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
                               var isOverBooking = false;
                               BookingEntity previous = null;
                               foreach (var booking in bookingsGroup.OrderBy(x => x.BookingStart))
                               {
                                   if (previous != null)
                                   {
                                       var daysForNextBooking = (booking.BookingStart - previous.BookingEnd).Days;
                                       if (daysForNextBooking <= updatedPreparationTimeInDays)
                                       {
                                           isOverBooking = true;
                                           break;
                                       }
                                   }

                                   previous = booking;
                               }

                               return isOverBooking;
                           });
        }

        private async Task<RentalEntity> UpdateRentalAndBookings(PutRentalRequest request, IEnumerable<BookingEntity> bookings)
        {
            _unitOfWork.BeginTransaction();

            RentalEntity updatedRental;

            try
            {
                updatedRental = await UpdateRental(request);
                await UpdateBookings(request, bookings);

                _unitOfWork.CommitTransaction();
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            return updatedRental;

            async Task<RentalEntity> UpdateRental(PutRentalRequest putRequest)
            {
                var rentalEntity = _mapper.Map<RentalEntity>(putRequest);
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
