using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Enums;
using VacationRental.Services.Interface.Models.Bookings;
using VacationRental.Services.Interface.Models.Calendar;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        #region Fields

        private readonly ICalendarService _calendarService;
        private readonly ICalendarValidationService _calendarValidationService;

        #endregion

        #region Constructor

        public CalendarController(ICalendarService calendarService,
                                  ICalendarValidationService calendarValidationService)
        {
            _calendarService = calendarService;
            _calendarValidationService = calendarValidationService;
        }

        #endregion

        #region Methods

        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Get a calendar" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The calendar", typeof(BookingViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Get(int rentalId, DateTime start, int nights)
        {
            var request = new GetCalendarRequest
            {
                RentalId = rentalId,
                StartDate = start,
                Nights = nights
            };

            var validationResult = _calendarValidationService.ValidateGetRequest(request);

            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _calendarService.GetAsync(request);

            switch (result.Status)
            {
                case ResponseStatus.RentalNotFound:
                    return BadRequest(VacationRentalConstants.RentalNotFoundErrorMessage);
                default:
                    return Ok(result.Result);
            }
        }

        #endregion
    }
}
