using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using StockMarket.Integration.Test.Fixtures;

namespace StockMarket.Integration.Test
{
    public class ExchangeControllerTest : IClassFixture<ExchangeApiFactory>
    {
        private readonly ExchangeApiFactory _factory;
        private readonly HttpClient _client;

        public ExchangeControllerTest(ExchangeApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetShouldReturnList()
        {
            var response = await _client.GetAsync("/exchange");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}