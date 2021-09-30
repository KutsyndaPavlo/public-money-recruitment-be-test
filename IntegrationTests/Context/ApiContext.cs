using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace IntegrationTests.Context
{
    internal static class ApiContext
    {
        /// <summary>
        /// Gets or sets the test server which hosts the acl API.
        /// </summary>
        public static TestServer Server { get; set; }

        /// <summary>
        /// Gets or sets an HTTP client configured to call the acl API.
        /// </summary>
        public static HttpClient Client { get; set; }
    }
}