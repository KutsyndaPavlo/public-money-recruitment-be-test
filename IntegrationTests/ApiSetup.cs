using IntegrationTests.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.IO;
using TechTalk.SpecFlow;
using VacationRental.Api;

namespace IntegrationTests.Steps
{
    [Binding]
    internal static class ApiSetup
    {
        [BeforeTestRun]
        public static void StartApi()
        {
            var appSettings = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..",
                "..",
                "..",
                "..",
                "VacationRental.Api",
                "appsettings");

            ApiContext.Server = new TestServer(
                new WebHostBuilder()
                    .UseEnvironment("Development")
                    .UseStartup<Startup>());

            ApiContext.Client = ApiContext.Server.CreateClient();
        }

        [AfterTestRun]
        public static void StopApi()
        {
            ApiContext.Client?.Dispose();
            ApiContext.Server?.Dispose();
        }
    }
}
