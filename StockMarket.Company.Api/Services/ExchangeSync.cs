using System.Threading.Tasks;
using MongoDB.Driver;
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
            var exchange = createdEvent.IntoEntity();
            var exists = await _context.Exchanges.Find(e => e.Id == exchange.Id).AnyAsync();
            
            if (!exists)
                await _context.Exchanges.InsertOneAsync(exchange);
            else
                await _context.Exchanges.ReplaceOneAsync(e => e.Id == exchange.Id, exchange);
        }

        private async Task OnDeletedEvent(ExchangeDeletedIntegrationEvent deletedEvent)
        {
            var exchangeId = deletedEvent.IntoId();
            await _context.Exchanges.DeleteOneAsync(e => e.Id == exchangeId);
        }
    }
}