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
        public async Task WhenGetCalendarByRentalIdFromAndNights555(string startDate, int nights)
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
            //Assert.AreEqual(table.Rows.Count, responseData.Dates.Count);

            var ee = table.Rows.ToList().GetEnumerator();




            var trResponse = new List<(DateTime date, int? bookingId, int? bookingUnit, int? preparationUnit)>();



            foreach (var date in responseData.Dates.OrderBy(x => x.Date))
            {
                foreach (var booking in date.Bookings.OrderBy(x => x.Id))
                {
                    trResponse.Add((date.Date, booking.Id, booking.Unit, default(int?)));
                }

                foreach (var preparation in date.PreparationTimes.OrderBy(x => x.Unit))
                {
                    trResponse.Add((date.Date, default(int?), default(int?), preparation.Unit));
                }

                if (!date.Bookings.Any() && !date.PreparationTimes.Any())
                {
                    trResponse.Add((date.Date, default(int?), default(int?), default(int?)));
                }
            }

            var confVal = table.Rows.Select(x => (DateTime.Parse(x[0]), GetVal(x[1]), GetVal(x[2]))).ToList();

            for (int i = 0; i < confVal.Count; i++) 
            {
                var row = confVal[i];
                var www = trResponse[i];

                if (row.Item1 != www.date ||  row.Item2 != www.bookingUnit || row.Item3 != www.preparationUnit)
                {
                    Assert.Fail($"Incorrect val");
                }
            }

            int? GetVal(string v)
            {
                if (string.IsNullOrWhiteSpace(v))
                {
                    return default(int?);
                }

                return int.Parse(v);
            }
           
            
        }
    }
}
