using AutoMapper;
using System;
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

        private readonly IRentalsRepository _rentalsRepository;
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public RentalsService(IRentalsRepository rentalsRepository,
            IBookingsRepository bookingsRepository,
            IMapper mapper)
        {
            _rentalsRepository = rentalsRepository;
            _bookingsRepository = bookingsRepository;
            _mapper = mapper;
        }

        #endregion

        #region Public methods

        public async Task<ServiceResponse<RentalViewModel>> GetByIdAsync(GetRentalRequest request)
        {
            var result = await _rentalsRepository.GetByIdAsync(request.RentalId);

            if (result == null)
            {
                return new ServiceResponse<RentalViewModel>
                {
                    Status = ResponseStatus.NotFound
                };
            }

            return new ServiceResponse<RentalViewModel>
            {
                Status = ResponseStatus.Success,
                Result = _mapper.Map<RentalViewModel>(await _rentalsRepository.GetByIdAsync(request.RentalId))
            };
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> AddAsync(RentalBindingModel units)
        {
            return new ServiceResponse<ResourceIdViewModel> { Result = _mapper.Map<ResourceIdViewModel>(await _rentalsRepository.AddAsync(_mapper.Map<RentalEntityCreate>(units))), Status = ResponseStatus.Success }; ;
        }

        public async Task<ServiceResponse<ResourceIdViewModel>> UpdateAsync(PutRentalRequest request)
        {
            var rental = await _rentalsRepository.GetByIdAsync(request.RentalId);

            if (rental == null)
            {

                return new ServiceResponse<ResourceIdViewModel>
                {
                    Status = ResponseStatus.NotFound
                };
            }
            var bookings = await _bookingsRepository.GetBookingsAsync(rental.Id, default(DateTime?), DateTime.Now);


            if (request.Units < rental.Units)
            {
                var uns = Enumerable.Range(request.Units + 1, rental.Units - request.Units).ToList();

                if (bookings.Any(x => uns.Any(y => y == x.UnitId)))
                {
                    throw new ApplicationException("test");
                }
            }


            foreach (var gr in bookings.GroupBy(x => x.UnitId))
            {
                BookingEntity st = null;
                gr.OrderBy(x => x.BookingStart).Aggregate(st, (previous, current) =>
                   {
                       if (previous != null && previous.PreparationEnd >= current.BookingStart)
                       {

                           throw new ApplicationException("test2");

                       }

                       current.PreparationEnd = current.PreparationStart.AddDays(request.PreparationTimeInDays - 1);

                       return current;
                   });
            }



            var rr = _mapper.Map<RentalEntity>(request);
            rr.Id = request.RentalId;

            foreach (var tt in bookings)
            {

                await _bookingsRepository.UpdateAsync(tt);

            }
            return new ServiceResponse<ResourceIdViewModel> { Result = _mapper.Map<ResourceIdViewModel>(await _rentalsRepository.UpdateAsync(rr)), Status = ResponseStatus.Success };

        }

        #endregion
    }
}
