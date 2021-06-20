using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using StockMarket.Sector.Api.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StockMarket.Sector.Api.Services
{
    public class EventBus
    {
        private readonly IAdvancedBus _bus;

        private readonly IExchange _sectorExchange;
        private readonly IExchange _companyExchange;

        private readonly IQueue _companyQueue;

        public EventBus(IOptions<EventBusConfig> config)
        {
            _bus = RabbitHutch.CreateBus(config.Value.ConnectionString).Advanced;

            _sectorExchange = _bus.ExchangeDeclare("Sector", ExchangeType.Fanout);
            _companyExchange = _bus.ExchangeDeclare("Company", ExchangeType.Fanout);

            _companyQueue = _bus.QueueDeclare("CompanyQueue_SectorApi");

            _bus.Bind(_companyExchange, _companyQueue, "*");
        }

       private async Task Publish<T>(IExchange exchange, T message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageWrap = new Message<string>(messageJson);
            await _bus.PublishAsync<string>(exchange, "*", false, messageWrap);
        }

        private void Subscribe<T>(IQueue queue, Func<T, Task> handler)
        {
            _bus.Consume<string>(queue, async (messageWrap, info) => {
                var messageJson = messageWrap.Body;
                var message = JsonSerializer.Deserialize<T>(messageJson);
                await handler(message);
            });
        }

        public async Task PublishSectorEvent(SectorIntegrationEvent message) =>
            await Publish(_sectorExchange, message);

        public void SubscribeCompanyEvent(Func<CompanyIntegrationEvent, Task> handler) =>
            Subscribe(_companyQueue, handler);
    }
}
