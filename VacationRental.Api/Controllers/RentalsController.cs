using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VacationRental.Api.Models;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : VacationRentalController
    {
        #region Fields

        private readonly IRentalsService _rentalsService;
        private const string RentalNotFoundMessage = "Rental not found";

        #endregion

        #region Constructor

        public RentalsController(IRentalsService rentalsService,
                                 ILogger<VacationRentalController> logger) : base(logger)
        {
            _rentalsService = rentalsService;
        }

        #endregion

        #region Actions

        [HttpGet]
        [Route("{rentalId:int}")]
        [SwaggerOperation(Tags = new[] { "Get rental by id" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The rental", typeof(RentalViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Rental not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public IActionResult Get(int rentalId)
        {
            return ProcessRequest(() =>
            {
                // validation

                var result = _rentalsService.GetById(rentalId);

                if (result.Status == ResponseStatus.NotFound)
                {
                    return NotFound(RentalNotFoundMessage);
                }

                return Ok(result.Result);
            });
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Create rental" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The created rental id", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public IActionResult Post(RentalBindingModel model)
        {
            return ProcessRequest(() =>
            {
                // validation

                var result = _rentalsService.Add(model);

                return CreatedAtAction(nameof(Get), new { rentalId = result.Result.Id }, result.Result);
            });
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        [SwaggerOperation(Tags = new[] { "Update rental" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The updated rental", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict during rental updating")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public IActionResult Put(int rentalId, [FromBody] RentalBindingModel model)
        {
            return ProcessRequest(() =>
            {
                // Validation
                var result = _rentalsService.Update(rentalId, model);

                if (result.Status == ResponseStatus.UpdateConflict)
                {
                    return Conflict();
                }

                return Ok(result.Result);
            }); 
        }

        #endregion
    }
}
