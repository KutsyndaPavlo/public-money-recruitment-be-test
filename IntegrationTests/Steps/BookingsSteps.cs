using IntegrationTests.Context;
using IntegrationTests.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace IntegrationTests.Features
{
    [Binding]
    public class BookingsSteps
    {
        private readonly ScenarioContext _context;

        public BookingsSteps(ScenarioContext context)
        {
            _context = context;
        }
        
        [Given(@"booking for ""(.*)"" for (.*) nights is created")]
        public async Task GivenBookingIsCreated(string bookingStartDate, int nights)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var requestData = new BookingBindingModel
            {
                RentalId = rental.Id,
                Start = DateTime.Parse(bookingStartDate),
                Nights  = nights
            };

            var response = await ApiContext.Client.PostAsync("api/v1/bookings", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                Assert.Fail($"Expected response status code to be {System.Net.HttpStatusCode.Created}, but got {response.StatusCode}.");
            }

            var responseData = JsonConvert.DeserializeObject<ResourceIdViewModel>(await response.Content.ReadAsStringAsync());

            _context.Set(responseData, "booking_create_response");
        }

        [When(@"booking for ""(.*)"" for (.*) nights is posted")]
        public async Task GivenBookingIsPosted(string bookingStartDate, int nights)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var requestData = new BookingBindingModel
            {
                RentalId = rental.Id,
                Start = DateTime.Parse(bookingStartDate),
                Nights = nights
            };

            var response = await ApiContext.Client.PostAsync("api/v1/bookings", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            _context.Set(response, "booking_create_response_message");
        }

        [When(@"get created booking")]
        public async Task WhenGetCreatedBooking()
        {
            var booking = _context.Get<ResourceIdViewModel>("booking_create_response");

            var response = await ApiContext.Client.GetAsync($"api/v1/bookings/{booking.Id}").ConfigureAwait(false);

            _context.Set(response, "booking_get_response");
        }

        [Then(@"the result should be (.*) and start date ""(.*)"" and nights (.*) and unit (.*)")]
        public async Task ThenTheResultShouldBe(int statusCode, string startDate, int nights, int unit)
        {
            var rentalResponse = _context.Get<ResourceIdViewModel>("rental_create_response");
            var bookingResponse = _context.Get<HttpResponseMessage>("booking_get_response");

            Assert.That(bookingResponse, Is.Not.Null);

            var actualStatus = (int)bookingResponse.StatusCode;

            if (actualStatus != statusCode)
            {
                var responseBody = await bookingResponse.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
            }

            var responseData = JsonConvert.DeserializeObject<BookingViewModel>(await bookingResponse.Content.ReadAsStringAsync());
                        
            Assert.AreEqual(responseData.RentalId, rentalResponse.Id);
            Assert.AreEqual(responseData.Start, DateTime.Parse(startDate));
            Assert.AreEqual(responseData.Nights, nights);
            Assert.AreEqual(responseData.Unit, unit);
        }

        [Then(@"the result should be (.*) and message ""(.*)""")]
        public async Task ThenTheResultShouldBeWithMessage(int statusCode, string errorMessage)
        {
            var bookingResponse = _context.Get<HttpResponseMessage>("booking_create_response_message");

            Assert.That(bookingResponse, Is.Not.Null);

            var actualStatus = (int)bookingResponse.StatusCode;

            if (actualStatus != statusCode)
            {
                var responseBody = await bookingResponse.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
            }
        }

        [When(@"Add a booking for ""(.*)"" for (.*) nights and rental id (.*)")]
        public async Task WhenAddABookingForForNightsAndRentalId(string date, int nights, int rentalId)
        {
            var requestData = new BookingBindingModel
            {
                RentalId = rentalId,
                Start = DateTime.Parse(date),
                Nights = nights
            };

            var response = await ApiContext.Client.PostAsync("api/v1/bookings", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            _context.Set(response, "booking_create_response_message");
        }

        [Then(@"the result of creating a booking should be (.*)")]
        public async Task ThenTheResultOfCreatingABookingShouldBe(int expectedStatusCode)
        {
            var response = _context.Get<HttpResponseMessage>("booking_create_response_message");

            Assert.That(response, Is.Not.Null);
            if ((int)response.StatusCode != expectedStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {expectedStatusCode}, but got {response.StatusCode }.\nResponse body:\n{responseBody}");
            }
        }
    }
}
