using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VacationRental.Api.Controllers;
using VacationRental.Services.Interface;
using VacationRental.Services.Interface.Validation;

namespace VacationRental.Api.Tests
{
    [TestFixture]
    public class RentalsControllerTests
    {
        #region Fields

        private Mock<IBookingValidationService> _bookingValidationServiceMock;
        private Mock<IBookingsService> _bookingsServiceMock;
        private BookingsController _bookingsController;

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            _bookingsServiceMock = new Mock<IBookingsService>();
            _bookingValidationServiceMock = new Mock<IBookingValidationService>();
            _bookingsController = new BookingsController(_bookingsServiceMock.Object, _bookingValidationServiceMock.Object);
        }

        #endregion

      

    }
}
