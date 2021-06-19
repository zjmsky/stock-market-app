using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using StockMarket.Integration.Test.Fixtures;

namespace StockMarket.Integration.Test
{
    public class AuthControllerTest : IClassFixture<AuthApiFactory>
    {
        private readonly AuthApiFactory _factory;
        private readonly HttpClient _client;

        public AuthControllerTest(AuthApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task LoginShouldErrorWhenInvalidCredentials()
        {
            var loginBody = new { username = "wronguser", password = "wrongpass" };
            var response = await _client.PostAsJsonAsync("/auth/login", loginBody);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterShouldSuccessWhenValid()
        {
            var registerBody = new { username = "correctuser", password = "StrongPass@123", email = "test@test.com" };
            var response = await _client.PostAsJsonAsync("/auth/register", registerBody);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}