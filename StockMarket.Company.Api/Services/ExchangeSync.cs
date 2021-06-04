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
            var createdEvent = integrationEvent as ExchangeCreatedIntegrationEvent;
            var deletedEvent = integrationEvent as ExchangeDeletedIntegrationEvent;

            if (createdEvent != null)
                await OnCreatedEvent(createdEvent);
            else if (deletedEvent != null)
                await OnDeletedEvent(deletedEvent);
        }

        private async Task OnCreatedEvent(ExchangeCreatedIntegrationEvent createdEvent)
        {
            var exchangeCode = createdEvent.ExchangeCode;
            var exchange = new Entities.Exchange(exchangeCode);
            var exists = await _context.Exchanges.Find(e => e.ExchangeCode == exchangeCode).AnyAsync();
            
            if (!exists)
                await _context.Exchanges.InsertOneAsync(exchange);
            else
                await _context.Exchanges.ReplaceOneAsync(e => e.ExchangeCode == exchangeCode, exchange);
        }

        private async Task OnDeletedEvent(ExchangeDeletedIntegrationEvent deletedEvent)
        {
            var exchangeCode = deletedEvent.ExchangeCode;
            await _context.Exchanges.DeleteOneAsync(e => e.ExchangeCode == exchangeCode);
        }
    }
}