using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : VacationRentalController
    {
        #region Fields 

        private readonly IBookingsService _bookingsService;
        private readonly IBookingValidatinService _bookingValidatinService;
        private const string BookingNotFoundErrorMessage = "Booking not found";

        #endregion

        #region Constructor

        public BookingsController(IBookingsService bookingsService,
                                  IBookingValidatinService bookingValidatinService,
                                  ILogger<VacationRentalController> logger) : base(logger)
        {
            _bookingsService = bookingsService;
            _bookingValidatinService = bookingValidatinService;
        }

        #endregion

        #region Actions

        [HttpGet]
        [Route("{bookingId:int}")]
        [SwaggerOperation(Tags = new[] { "GetAsync booking by id" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The booking", typeof(BookingViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Get(int bookingId)
        {
            return await ProcessRequestAsync(async () =>
            {
                var request = new GetBookingRequest { BookingId = bookingId };

                var validationResult = _bookingValidatinService.ValidateGetRequest(request);
                if (validationResult.Status == ResponseStatus.ValidationFailed)
                {
                    return BadRequest(validationResult.Result);
                }

                var result = await _bookingsService.GetAsync(request);
                if (result.Status == ResponseStatus.NotFound)
                {
                    return NotFound(BookingNotFoundErrorMessage);
                }

                return Ok(result.Result);
            });
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Create booking" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The created booking id", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict during adding a booking")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Post(BookingBindingModel request)
        {
            return await ProcessRequestAsync(async () =>
            {
                var validationResult = await _bookingValidatinService.ValidatePostRequestAsync(request);
                if (validationResult.Status == ResponseStatus.ValidationFailed)
                {
                    return BadRequest(validationResult.Result);
                }

                var result = await _bookingsService.AddAsync(request);
                if (result.Status == ResponseStatus.UpdateConflict)
                {
                    return Conflict();
                }

                return CreatedAtAction(nameof(Get), new { bookingId = result.Result.Id }, result.Result);
            });
        }

        #endregion
    }
}
