using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Models;

namespace StockMarket.Exchange.Api.Services
{
    public class ListingSync
    {
        private readonly DatabaseContext _context;

        public ListingSync(DatabaseContext context, EventBus events)
        {
            _context = context;
            events.Subscribe<IListingIntegrationEvent>("ExchangeApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(IListingIntegrationEvent integrationEvent)
        {
            var creationEvent = integrationEvent as ListingCreationEvent;
            var deletionEvent = integrationEvent as ListingDeletionEvent;

            if (creationEvent != null)
                await OnCreationEvent(creationEvent);
            else if (deletionEvent != null)
                await OnDeletionEvent(deletionEvent);
        }

        private async Task OnCreationEvent(ListingCreationEvent creationEvent)
        {
            var listing = creationEvent.ToEntity();
            var exists = await _context.Listings.Find(l => l.Id == listing.Id).AnyAsync();

            if (!exists)
                await _context.Listings.InsertOneAsync(listing);
            else
                await _context.Listings.ReplaceOneAsync(l => l.Id == listing.Id, listing);
        }

        private async Task OnDeletionEvent(ListingDeletionEvent deletionEvent)
        {
            var listingId = deletionEvent.ToId();
            await _context.Listings.DeleteOneAsync(l => l.Id == listingId);
        }
    }
}