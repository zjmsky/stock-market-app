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
            events.Subscribe<ExchangeIntegrationEvent>("ListingApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(ExchangeIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdationEvent())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletionEvent())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(ExchangeIntegrationEvent updationEvent)
        {
            var exchange = updationEvent.ToEntity();
            await _repo.InsertOrReplaceOneUnchecked(exchange);
        }

        private async Task OnDeletionEvent(ExchangeIntegrationEvent deletionEvent)
        {
            var code = deletionEvent.ExchangeCode;
            var exchange = await _repo.FindOneByCode(code);
            await _repo.DeleteOneUnchecked(exchange);
        }
    }
}