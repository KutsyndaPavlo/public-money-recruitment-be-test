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
    public class CalendarService : ICalendarService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private int currentNight = 0;

        #endregion

        #region Constructor

        public CalendarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task<ServiceResponse<CalendarViewModel>> GetAsync(GetCalendarRequest request)
        {
            var rental = await _unitOfWork.RentalsRepository.GetByIdAsync(request.RentalId);

            if (rental == null)
            {
                return new ServiceResponse<CalendarViewModel>
                {
                    Status = ResponseStatus.RentalNotFound
                };
            }

            var bookings = await _unitOfWork.BookingsRepository.GetBookingsAsync(request.RentalId,
                                                                                                       request.StartDate,
                                                                                                       request.StartDate.AddDays(request.Nights));
            var result = new CalendarViewModel
            {
                RentalId = request.RentalId,
                Dates = Enumerable.Range(currentNight, request.Nights)
                                  .Select(nightNumber => GetCalendarDateViewModel(request.StartDate.Date.AddDays(nightNumber), bookings))
                                  .ToList()
            };

            return new ServiceResponse<CalendarViewModel>
            {
                Result = result,
                Status = ResponseStatus.Success
            };
        }

        #endregion

        #region Private methods

        private CalendarDateViewModel GetCalendarDateViewModel(DateTime date, IEnumerable<BookingEntity> bookings)
        {
            return new CalendarDateViewModel
            {
                Date = date,
                Bookings = GetDateBookings(),
                PreparationTimes = GetPreparationTimes()
            };

            List<CalendarBookingViewModel> GetDateBookings()
            {
                return bookings
                    .Where(x => x.BookingStart <= date.Date && x.BookingEnd >= date.Date)
                    .Select(x =>
                        new CalendarBookingViewModel
                        {
                            Id = x.Id,
                            Unit = x.UnitId
                        })
                    .OrderBy(x => x.Unit)
                    .ToList();
            }

            List<CalendarPreparationTimeViewModel> GetPreparationTimes()
            {
                return bookings
                    .Where(x => x.PreparationStart <= date.Date && x.PreparationEnd >= date.Date)
                    .Select(x =>
                        new CalendarPreparationTimeViewModel
                        {
                            Unit = x.UnitId
                        })
                    .OrderBy(x => x.Unit)
                    .ToList();
            }
        }

        #endregion
    }
}
