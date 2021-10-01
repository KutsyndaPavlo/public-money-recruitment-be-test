using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : VacationRentalController
    {
        #region Fields

        private readonly ICalendarService _calendarService;
        private readonly ICalendarValidationService _calendarValidationService;

        #endregion

        #region Constructor

        public CalendarController(ICalendarService calendarService,
                                  ICalendarValidationService calendarValidationService,
                                  ILogger<VacationRentalController> logger) : base(logger)
        {
            _calendarService = calendarService;
            _calendarValidationService = calendarValidationService;
        }

        #endregion

        #region Methods

        [HttpGet]
        [SwaggerOperation(Tags = new[] { "GetAsync calendar" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The calendar", typeof(BookingViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Get(int rentalId, DateTime start, int nights)
        {
            return await ProcessRequestAsync(async () =>
            {
                var request = new GetCalendarRequest
                {
                    RentalId = rentalId,
                    StartDate = start,
                    Nights = nights
                };

                var validationResult = await _calendarValidationService.ValidateGetRequestAsync(request);

                if (validationResult.Status == ResponseStatus.ValidationFailed)
                {
                    return BadRequest(validationResult.Result);
                }

                var result = await _calendarService.GetAsync(request);

                return Ok(result.Result);
            });
        }

        #endregion
    }
}
