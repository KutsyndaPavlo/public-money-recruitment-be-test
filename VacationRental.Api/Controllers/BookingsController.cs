using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Shared;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        #region Fields 

        private readonly IBookingsService _bookingsService;
        private readonly IBookingValidatinService _bookingValidationService;

        #endregion

        #region Constructor

        public BookingsController(IBookingsService bookingsService,
                                  IBookingValidatinService bookingValidationService)
        {
            _bookingsService = bookingsService;
            _bookingValidationService = bookingValidationService;
        }

        #endregion

        #region Actions

        [HttpGet]
        [Route("{bookingId:int}")]
        [SwaggerOperation(Tags = new[] { "Get a booking by id" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The booking", typeof(BookingViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Get(int bookingId)
        {
            var request = new GetBookingRequest { BookingId = bookingId };

            var validationResult = _bookingValidationService.ValidateGetRequest(request);
            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _bookingsService.GetAsync(request);
            if (result.Status == ResponseStatus.BookingNotFound)
            {
                return NotFound(VacationRentalConstants.BookingNotFoundErrorMessage);
            }

            return Ok(result.Result);
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Create a booking" })]
        [SwaggerResponse(StatusCodes.Status201Created, "The created booking id", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict during adding a booking")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Post(BookingBindingModel request)
        {
            var validationResult = _bookingValidationService.ValidatePostRequest(request);
            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _bookingsService.AddAsync(request);

            switch (result.Status)
            {
                case ResponseStatus.RentalNotFound:
                    return BadRequest(VacationRentalConstants.RentalNotFoundErrorMessage);
                case ResponseStatus.Conflict:
                    return Conflict(VacationRentalConstants.BookingAddingConflictErrorMessage);
                default:
                    return CreatedAtAction(nameof(Get), new { bookingId = result.Result.Id }, result.Result);
            }
        }

        #endregion
    }
}
