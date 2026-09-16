// Pseudocode / Plan (detailed):
// 1. Create an xUnit test class that uses WebApplicationFactory<Program> to host the minimal API in-memory.
// 2. Use the factory to create an HttpClient.
// 3. Send a GET request to "/weatherforecast".
// 4. Parse the JSON response as a JsonDocument to avoid requiring the exact WeatherForecast type.
// 5. Count the elements in the returned JSON array.
// 6. Assert that the count equals 10 (this is intentionally incorrect because the API returns 5 items).
// 7. This assertion should fail when the test is executed, producing a failing unit test as requested.

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ProductApi.Tests
{
    public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WeatherForecastTests(WebApplicationFactory<Program> factory) =>
            _factory = factory;

        [Fact]
        public async Task GetWeatherForecast_ReturnsTenItems()
        {
            // Arrange
            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert basic success
            response.EnsureSuccessStatusCode();

            // Read and parse JSON without depending on the app's model types
            var payload = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            // Count array elements (API currently returns 5)
            var count = root.GetArrayLength();

            // Intentionally assert an incorrect expected value to produce a failing test
            Assert.Equal(10, count);
        }
    }
}