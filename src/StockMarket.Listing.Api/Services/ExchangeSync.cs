using System.Threading.Tasks;
using StockMarket.Listing.Api.Models;

namespace StockMarket.Listing.Api.Services
{
    public class ExchangeSync
    {
        private readonly ExchangeRepo _repo;

        public ExchangeSync(ExchangeRepo repo, EventBus events)
        {
            _repo = repo;
            events.SubscribeExchangeEvent((ev) => OnIntegrationEvent(ev));
        }

        private async Task OnIntegrationEvent(ExchangeIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdation())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletion())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(ExchangeIntegrationEvent updationEvent)
        {
            var exchange = updationEvent.ToEntity();
            var result = await _repo.InsertOrReplaceOneUnchecked(exchange);
        }

        private async Task OnDeletionEvent(ExchangeIntegrationEvent deletionEvent)
        {
            var code = deletionEvent.ExchangeCode;
            var exchange = await _repo.FindOneByCode(code);
            var result = await _repo.DeleteOneUnchecked(exchange);
        }
    }
}