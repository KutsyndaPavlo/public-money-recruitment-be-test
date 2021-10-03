using IntegrationTests.Context;
using IntegrationTests.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace IntegrationTests.Features
{
    [Binding]
    public class CalendarSteps
    {
        private readonly ScenarioContext _context;

        public CalendarSteps(ScenarioContext context)
        {
            _context = context;
        }

        [When(@"get calendar by rental id from ""(.*)"" and (.*) nights")]
        public async Task WhenGetCalendarByRentalIdFromAndNights(string startDate, int nights)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var response = await ApiContext.Client.GetAsync($"api/v1/calendar/?rentalId={rental.Id}&start={startDate}&nights={nights}").ConfigureAwait(false);

            _context.Set(response, "calendar_get_response_message");
        }

        [When(@"get calendar by rental id after updates from ""(.*)"" and (.*) nights")]
        public async Task WhenGetCalendarByRentalIdFromAndNights_1(string startDate, int nights)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_update_response");

            var response = await ApiContext.Client.GetAsync($"api/v1/calendar/?rentalId={rental.Id}&start={startDate}&nights={nights}").ConfigureAwait(false);

            _context.Set(response, "calendar_get_response_message");
        }

        [Then(@"the result should be (.*) and data")]
        public async Task ThenTheResultShouldBe(int statusCode, Table table)
        {

            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");
            var response = _context.Get<HttpResponseMessage>("calendar_get_response_message");

            Assert.That(response, Is.Not.Null);

            var actualStatus = (int)response.StatusCode;

            if (actualStatus != statusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
            }
            var responseData = JsonConvert.DeserializeObject<CalendarViewModel>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(rental.Id, responseData.RentalId);

            var actualValues = new List<(DateTime date, int? bookingId, int? bookingUnit, int? preparationUnit)>();

            foreach (var date in responseData.Dates.OrderBy(x => x.Date))
            {
                foreach (var booking in date.Bookings.OrderBy(x => x.Id))
                {
                    actualValues.Add((date.Date, booking.Id, booking.Unit, default(int?)));
                }

                foreach (var preparation in date.PreparationTimes.OrderBy(x => x.Unit))
                {
                    actualValues.Add((date.Date, default(int?), default(int?), preparation.Unit));
                }

                if (!date.Bookings.Any() && !date.PreparationTimes.Any())
                {
                    actualValues.Add((date.Date, default(int?), default(int?), default(int?)));
                }
            }

            var expectedValues = table.Rows.Select(x => (DateTime.Parse(x[0]), GetIntValue(x[1]), GetIntValue(x[2]))).ToList();

            for (int i = 0; i < expectedValues.Count; i++) 
            {
                var expectedValue = expectedValues[i];
                var actualValue = actualValues[i];

                if (expectedValue.Item1 != actualValue.date ||  expectedValue.Item2 != actualValue.bookingUnit || expectedValue.Item3 != actualValue.preparationUnit)
                {
                    Assert.Fail($"Calendar data is incorrect");
                }
            }

            int? GetIntValue(string v)
            {
                return string.IsNullOrWhiteSpace(v) ? default(int?) : int.Parse(v);
            }  
        }
    }
}
