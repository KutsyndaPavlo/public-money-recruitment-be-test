﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : VacationRentalController
    {
        #region Fields

        private readonly IRentalsService _rentalsService;
        private readonly IRentalValidationService _rentalValidationService;
        private const string RentalNotFoundMessage = "Rental not found";

        #endregion

        #region Constructor

        public RentalsController(IRentalsService rentalsService,
                                 IRentalValidationService rentalValidationService,
                                 ILogger<VacationRentalController> logger) : base(logger)
        {
            _rentalsService = rentalsService;
            _rentalValidationService = rentalValidationService;
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
                var request = new GetRentalRequest
                {
                    RentalId = rentalId
                };

                var validationResult = _rentalValidationService.ValidateGetRequest(request);
                if (validationResult.Status == ResponseStatus.ValidationFailed)
                {
                    return BadRequest(validationResult.Result);
                }

                var result = _rentalsService.GetById(request);
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
        public IActionResult Post(RentalBindingModel request)
        {
            return ProcessRequest(() =>
            {
                var validationResult = _rentalValidationService.ValidatePostRequest(request);
                if (validationResult.Status == ResponseStatus.ValidationFailed)
                {
                    return BadRequest(validationResult.Result);
                }

                var result = _rentalsService.Add(request);

                return CreatedAtAction(nameof(Get), new { rentalId = result.Result.Id }, result.Result);
            });
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        [SwaggerOperation(Tags = new[] { "Update rental" })]
        [SwaggerResponse(StatusCodes.Status200OK, "The updated rental", typeof(ResourceIdViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Rental not found", typeof(string))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict during rental updating")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something has gone wrong.", typeof(string))]
        public IActionResult Put(int rentalId, [FromBody] RentalBindingModel model)
        {
            return ProcessRequest(() =>
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

                var result = _rentalsService.Update(request);

                switch (result.Status) 
                {
                    case ResponseStatus.NotFound:
                        return NotFound();
                    case ResponseStatus.UpdateConflict:
                        return Conflict();
                    default:
                        return Ok(result.Result);
                }
            }); 
        }

        #endregion
    }
}
