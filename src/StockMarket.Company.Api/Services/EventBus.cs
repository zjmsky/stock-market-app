using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using StockMarket.Company.Api.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StockMarket.Company.Api.Services
{
    public class EventBus
    {
        private readonly IAdvancedBus _bus;

        private readonly IExchange _sectorExchange;
        private readonly IExchange _companyExchange;
        private readonly IExchange _listingExchange;

        private readonly IQueue _sectorQueue;
        private readonly IQueue _listingQueue;

        public EventBus(IOptions<EventBusConfig> config)
        {
            _bus = RabbitHutch.CreateBus(config.Value.ConnectionString).Advanced;

            _sectorExchange = _bus.ExchangeDeclare("Sector", ExchangeType.Fanout);
            _companyExchange = _bus.ExchangeDeclare("Company", ExchangeType.Fanout);
            _listingExchange = _bus.ExchangeDeclare("Listing", ExchangeType.Fanout);

            _sectorQueue = _bus.QueueDeclare("SectorQueue_CompanyApi");
            _listingQueue = _bus.QueueDeclare("ListingQueue_CompanyApi");

            _bus.Bind(_sectorExchange, _sectorQueue, "*");
            _bus.Bind(_listingExchange, _listingQueue, "*");
        }

        private async Task Publish<T>(IExchange exchange, T message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageWrap = new Message<string>(messageJson);
            await _bus.PublishAsync<string>(exchange, "*", false, messageWrap);
        }

        private void Subscribe<T>(IQueue queue, Func<T, Task> handler)
        {
            _bus.Consume<string>(queue, async (messageWrap, info) =>
            {
                var messageJson = messageWrap.Body;
                var message = JsonSerializer.Deserialize<T>(messageJson);
                await handler(message);
            });
        }

        public async Task PublishCompanyEvent(CompanyIntegrationEvent message) =>
            await Publish(_companyExchange, message);

        public void SubscribeSectorEvent(Func<SectorIntegrationEvent, Task> handler) =>
            Subscribe(_sectorQueue, handler);

        public void SubscribeListingEvent(Func<ListingIntegrationEvent, Task> handler) =>
            Subscribe(_listingQueue, handler);
    }
}