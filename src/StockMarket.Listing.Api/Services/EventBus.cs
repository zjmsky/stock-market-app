using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EasyNetQ;
using EasyNetQ.Topology;
using StockMarket.Listing.Api.Models;

namespace StockMarket.Listing.Api.Services
{
    public class EventBus
    {
        private readonly IAdvancedBus _bus;

        private readonly IExchange _exchangeExchange;
        private readonly IExchange _companyExchange;
        private readonly IExchange _listingExchange;
        
        private readonly IQueue _exchangeQueue;
        private readonly IQueue _companyQueue;

        public EventBus(IOptions<EventBusConfig> config)
        {
            _bus = RabbitHutch.CreateBus(config.Value.ConnectionString).Advanced;
            
            _exchangeExchange = _bus.ExchangeDeclare("Exchange", ExchangeType.Fanout);
            _companyExchange = _bus.ExchangeDeclare("Company", ExchangeType.Fanout);
            _listingExchange = _bus.ExchangeDeclare("Listing", ExchangeType.Fanout);

            _exchangeQueue = _bus.QueueDeclare("ExchangeQueue_ListingApi");
            _companyQueue = _bus.QueueDeclare("CompanyQueue_ListingApi");
            
            _bus.Bind(_exchangeExchange, _exchangeQueue, "*");
            _bus.Bind(_companyExchange, _companyQueue, "*");
        }

        public async Task PublishListingEvent(ListingIntegrationEvent integrationEvent)
        {
            var eventJson = System.Text.Json.JsonSerializer.Serialize(integrationEvent);
            var message = new Message<string>(eventJson);
            await _bus.PublishAsync(_listingExchange, "*", false, message);
        }

        public void SubscribeExchangeEvent(Func<ExchangeIntegrationEvent, Task> onMessage) 
        {
            _bus.Consume<string>(_exchangeQueue, async (message, info) => {
                System.Console.WriteLine("found : " + message.Body);
                var integrationEvent = System.Text.Json.JsonSerializer.Deserialize<ExchangeIntegrationEvent>(message.Body);
                await onMessage(integrationEvent);
            });
        }

        public void SubscribeCompanyEvent(Func<CompanyIntegrationEvent, Task> onMessage) 
        {
            _bus.Consume<string>(_companyQueue, async (message, info) => {
                var integrationEvent = System.Text.Json.JsonSerializer.Deserialize<CompanyIntegrationEvent>(message.Body);
                await onMessage(integrationEvent);
            });
        }
    }
}
