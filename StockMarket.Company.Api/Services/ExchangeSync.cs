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
            var creationEvent = integrationEvent as ExchangeCreationEvent;
            var deletionEvent = integrationEvent as ExchangeDeletionEvent;

            if (creationEvent != null)
                await OnCreationEvent(creationEvent);
            else if (deletionEvent != null)
                await OnDeletionEvent(deletionEvent);
        }

        private async Task OnCreationEvent(ExchangeCreationEvent creationEvent)
        {
            var exchange = creationEvent.ToEntity();
            var exists = await _context.Exchanges.Find(e => e.Id == exchange.Id).AnyAsync();

            if (!exists)
                await _context.Exchanges.InsertOneAsync(exchange);
            else
                await _context.Exchanges.ReplaceOneAsync(e => e.Id == exchange.Id, exchange);
        }

        private async Task OnDeletionEvent(ExchangeDeletionEvent deletionEvent)
        {
            var exchangeId = deletionEvent.ToId();
            await _context.Exchanges.DeleteOneAsync(e => e.Id == exchangeId);
        }
    }
}