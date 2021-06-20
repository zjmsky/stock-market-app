using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using StockMarket.Exchange.Api.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StockMarket.Exchange.Api.Services
{
    public class EventBus
    {
        private readonly IAdvancedBus _bus;

        private readonly IExchange _exchangeExchange;
        private readonly IExchange _listingExchange;

        private readonly IQueue _listingQueue;

        public EventBus(IOptions<EventBusConfig> config)
        {
            _bus = RabbitHutch.CreateBus(config.Value.ConnectionString).Advanced;

            _exchangeExchange = _bus.ExchangeDeclare("Exchange", ExchangeType.Fanout);
            _listingExchange = _bus.ExchangeDeclare("Listing", ExchangeType.Fanout);

            _listingQueue = _bus.QueueDeclare("ListingQueue_ExchangeApi");

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
            _bus.Consume<string>(queue, async (messageWrap, info) => {
                var messageJson = messageWrap.Body;
                var message = JsonSerializer.Deserialize<T>(messageJson);
                await handler(message);
            });
        }

        public async Task PublishExchangeEvent(ExchangeIntegrationEvent message) =>
            await Publish(_exchangeExchange, message);

        public void SubscribeListingEvent(Func<ListingIntegrationEvent, Task> handler) =>
            Subscribe(_listingQueue, handler);
    }
}
