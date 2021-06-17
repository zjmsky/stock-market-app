using System.Threading.Tasks;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class ListingSync
    {
        private readonly ListingRepo _repo;

        public ListingSync(ListingRepo repo, EventBus events)
        {
            _repo = repo;
            events.Subscribe<ListingIntegrationEvent>("CompanyApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(ListingIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdationEvent())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletionEvent())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(ListingIntegrationEvent updationEvent)
        {
            var listing = updationEvent.ToEntity();
            await _repo.InsertOrReplaceOneUnchecked(listing);
        }

        private async Task OnDeletionEvent(ListingIntegrationEvent deletionEvent)
        {
            var exchange = deletionEvent.ExchangeCode;
            var ticker = deletionEvent.TickerSymbol;
            var listing = await _repo.FindOneByTicker(exchange, ticker);
            await _repo.DeleteOneUnchecked(listing);
        }
    }
}