using IntegrationTests.Context;
using IntegrationTests.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace IntegrationTests.Steps
{
    [Binding]
    public class RentalSteps
    {
        private readonly ScenarioContext _context;

        public RentalSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"rental with (.*) units and preparation time (.*) is created")]
        public async Task GivenRentalIsCreated(int units, int preparationTime)
        {
            var requestData = new RentalBindingModel
            {
                Units = units,
                PreparationTimeInDays = preparationTime
            };

            var response = await ApiContext.Client.PostAsync("api/v1/rentals", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            Assert.That(response, Is.Not.Null);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                Assert.Fail($"Expected response status code to be {System.Net.HttpStatusCode.Created}, but got {response.StatusCode}.");
            }

            var responseData = JsonConvert.DeserializeObject<ResourceIdViewModel>(await response.Content.ReadAsStringAsync());

            _context.Set(responseData, "rental_create_response");
        }

        [When(@"get rental by id")]
        public async Task WhenGetRentalById()
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var response = await ApiContext.Client.GetAsync($"api/v1/rentals/{rental.Id}").ConfigureAwait(false);

            _context.Set(response, "rental_get_response");
        }

        [When(@"get updated rental by id")]
        public async Task WhenGetUpdatedRentalById()
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_update_response");

            var response = await ApiContext.Client.GetAsync($"api/v1/rentals/{rental.Id}").ConfigureAwait(false);

            _context.Set(response, "rental_get_response");
        }

        [Then(@"the rental update result should be (.*)")]
        public async Task ThenTheRentalUpdateResultShouldBe(int statusCode)
        {
            var responseMessage = _context.Get<HttpResponseMessage>("rental_update_response");

            Assert.That(responseMessage, Is.Not.Null);

            var actualStatus = (int)responseMessage.StatusCode;

            if (actualStatus != statusCode)
            {
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
            }
        }

        [Then(@"the result should be (.*) and units (.*) and preparation time (.*)")]
        public async Task ThenTheResultShouldBe(int statusCode, int units, int preparationTime)
        {
            var responseMessage = _context.Get<HttpResponseMessage>("rental_get_response");

            Assert.That(responseMessage, Is.Not.Null);

            var actualStatus = (int)responseMessage.StatusCode;

            if (actualStatus != statusCode)
            {
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
            }

            var rental = JsonConvert.DeserializeObject<RentalViewModel>(await responseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(units, rental.Units);

            Assert.AreEqual(preparationTime, rental.PreparationTimeInDays);
        }


        //[Then(@"the update result should be (.*) and units (.*) and preparation time (.*)")]
        //public async Task ThenTheUpdateResultShouldBeAndUnitsAndPreparationTime(int statusCode, int units, int preparationTime)
        //{
        //    var responseMessage = _context.GetAsync<HttpResponseMessage>("rental_get_response");

        //    Assert.That(responseMessage, Is.Not.Null);

        //    var actualStatus = (int)responseMessage.StatusCode;

        //    if (actualStatus != statusCode)
        //    {
        //        var responseBody = await responseMessage.Content.ReadAsStringAsync();
        //        Assert.Fail($"Expected response status code to be {statusCode}, but got {actualStatus}.\nResponse body:\n{responseBody}");
        //    }

        //    var rental = JsonConvert.DeserializeObject<RentalViewModel>(await responseMessage.Content.ReadAsStringAsync());

        //    Assert.AreEqual(units, rental.Units);

        //    Assert.AreEqual(preparationTime, rental.PreparationTimeInDays);
        //}

        [Given(@"update1 rental by setting (.*) units and preparation time (.*)")]
        public async Task GivenUpdateRentalBySettingUnitsAndPreparationTime(int units, int preparationTime)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var requestData = new RentalBindingModel
            {
                Units = units,
                PreparationTimeInDays = preparationTime
            };

            var response = await ApiContext.Client.PutAsync($"api/v1/rentals/{rental.Id}", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            Assert.That(response, Is.Not.Null);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Assert.Fail($"Expected response status code to be {System.Net.HttpStatusCode.OK}, but got {response.StatusCode}.");
            }

            var responseData = JsonConvert.DeserializeObject<ResourceIdViewModel>(await response.Content.ReadAsStringAsync());

            _context.Set(responseData, "rental_update_response");
        }


        [When(@"update rental by setting (.*) units and preparation time (.*)")]
        [Given(@"update rental by setting (.*) units and preparation time (.*)")]
        public async Task GivenUpdateRentalBySettingUnitsAndPreparationTime444(int units, int preparationTime)
        {
            var rental = _context.Get<ResourceIdViewModel>("rental_create_response");

            var requestData = new RentalBindingModel
            {
                Units = units,
                PreparationTimeInDays = preparationTime
            };

            var response = await ApiContext.Client.PutAsync($"api/v1/rentals/{rental.Id}", JsonContent.Create(requestData))
                .ConfigureAwait(false);

            Assert.That(response, Is.Not.Null);

            _context.Set(response, "rental_update_response");
        }
    }
}
