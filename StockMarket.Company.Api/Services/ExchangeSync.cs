using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class ExchangeSync
    {
        private readonly DatabaseContext _context;

        public ExchangeSync(DatabaseContext context, EventBus events)
        {
            _context = context;
            events.Subscribe<IExchangeIntegrationEvent>("CompanyApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(IExchangeIntegrationEvent integrationEvent)
        {
            var createdEvent = integrationEvent as ExchangeCreatedEvent;
            var deletedEvent = integrationEvent as ExchangeDeletedEvent;

            if (createdEvent != null)
                await OnCreatedEvent(createdEvent);
            else if (deletedEvent != null)
                await OnDeletedEvent(deletedEvent);
        }

        private async Task OnCreatedEvent(ExchangeCreatedEvent createdEvent)
        {
            var exchange = new Entities.Exchange
            {
                Id = new ObjectId(createdEvent.Id),
                ExchangeCode = createdEvent.ExchangeCode,
            };

            var exists = await _context.Exchanges.Find(e => e.Id == exchange.Id).AnyAsync();
            if (!exists)
                await _context.Exchanges.InsertOneAsync(exchange);
            else
                await _context.Exchanges.ReplaceOneAsync(e => e.Id == exchange.Id, exchange);
        }

        private async Task OnDeletedEvent(ExchangeDeletedEvent deletedEvent)
        {
            var exchangeId = new ObjectId(deletedEvent.Id);
            await _context.Exchanges.DeleteOneAsync(e => e.Id == exchangeId);
        }
    }
}