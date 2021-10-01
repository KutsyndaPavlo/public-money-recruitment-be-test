using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using VacationRental.Services.Constants;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        #region Fields

        private readonly IRentalsService _rentalsService;
        private readonly IRentalValidationService _rentalValidationService;

        #endregion

        #region Constructor

        public RentalsController(IRentalsService rentalsService,
                                 IRentalValidationService rentalValidationService)
        {
            _rentalsService = rentalsService;
            _rentalValidationService = rentalValidationService;
        }

        #endregion

        #region Actions

        [HttpGet]
        [Route("{rentalId:int}")]
        [SwaggerOperation(Tags = new[] { "GetAsync rental by id" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The rental", typeof(RentalViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Rental not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Get(int rentalId)
        {
            var request = new GetRentalRequest
            {
                RentalId = rentalId
            };

            var validationResult = _rentalValidationService.ValidateGetRequest(request);
            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _rentalsService.GetByIdAsync(request);
            if (result.Status == ResponseStatus.RentalNotFound)
            {
                return NotFound(VacationRentalConstants.RentalNotFoundErrorMessage);
            }

            return Ok(result.Result);
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Create rental" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The created rental id", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Post(RentalBindingModel request)
        {
            var validationResult = _rentalValidationService.ValidatePostRequest(request);
            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _rentalsService.AddAsync(request);

            return CreatedAtAction(nameof(Get), new { rentalId = result.Result.Id }, result.Result);
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        [SwaggerOperation(Tags = new[] { "UpdateAsync rental" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The updated rental", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Rental not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict during rental updating")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public async Task<IActionResult> Put(int rentalId, [FromBody] RentalBindingModel model)
        {
            var request = new PutRentalRequest
            {
                RentalId = rentalId,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            };

            var validationResult = _rentalValidationService.ValidatePutRequest(request);
            if (validationResult.Status == ResponseStatus.ValidationFailed)
            {
                return BadRequest(validationResult.Result);
            }

            var result = await _rentalsService.UpdateAsync(request);

            switch (result.Status)
            {
                case ResponseStatus.RentalNotFound:
                    return NotFound(VacationRentalConstants.RentalNotFoundErrorMessage);
                case ResponseStatus.Conflict:
                    return Conflict(VacationRentalConstants.RentalUpdatingConflictErrorMessage);
                default:
                    return Ok(result.Result);
            }
        }

        #endregion
    }
}
