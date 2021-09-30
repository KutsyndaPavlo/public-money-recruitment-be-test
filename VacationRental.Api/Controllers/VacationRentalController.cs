using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace VacationRental.Api.Controllers
{
    public class VacationRentalController : ControllerBase
    {
        #region Fields

        private const string errorMessage = "Something has gone wrong.";
        protected ILogger<VacationRentalController> _logger { get; set; }

        #endregion

        #region Constructor

        public VacationRentalController(ILogger<VacationRentalController> logger) 
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        protected IActionResult ProcessRequest(Func<IActionResult> func) 
        {
            try
            {
                return func();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());

                return new ObjectResult(errorMessage) 
                { 
                    StatusCode = StatusCodes.Status500InternalServerError 
                };
            } 
        }

        #endregion
    }
}
