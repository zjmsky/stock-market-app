using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EasyNetQ;
using StockMarket.Listing.Api.Models;

namespace StockMarket.Listing.Api.Services
{
    public class EventBus
    {
        private readonly IBus _bus;

        public EventBus(IOptions<EventBusConfig> config)
        {
            var bus = RabbitHutch.CreateBus(config.Value.ConnectionString);

            _bus = bus;
        }

        public async Task Publish<T>(T message)
        {
            await _bus.PubSub.PublishAsync<T>(message).ConfigureAwait(false);
        }

        public ISubscriptionResult Subscribe<T>(string subId, Func<T, Task> onMessage)
        {
            return _bus.PubSub.Subscribe<T>(subId, onMessage);
        }
    }
}
