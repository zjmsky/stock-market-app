using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Services
{
    public class ListingRepo
    {
        private readonly DatabaseContext _context;
        private readonly EventBus _events;

        public ListingRepo(DatabaseContext context, EventBus events)
        {
            _context = context;
            _events = events;
        }

        private async Task<bool> ValidateReferences(ListingEntity listing)
        {
            // validate company reference
            var companyExists = await _context.Companies
                .Find(c => c.CompanyCode == listing.CompanyCode)
                .AnyAsync();
            if (!companyExists) return false;

            // validate exchange reference
            var exchangeExists = await _context.Exchanges
                .Find(e => e.ExchangeCode == listing.ExchangeCode)
                .AnyAsync();
            if (!exchangeExists) return false;

            return true;
        }

        private async Task<bool> AssertNoReferences(ListingEntity listing)
        {
            // none for now
            await Task.Yield();
            return true;
        }

        private async Task<bool> InsertOrReplaceOneUnchecked(ListingEntity listing)
        {
            // check if entry exists already
            var listingExists = await _context.Listings
                .Find(l => l.IsSimilar(listing))
                .AnyAsync();

            try
            {
                if (!listingExists)
                    await _context.Listings.InsertOneAsync(listing);
                else
                    await _context.Listings.ReplaceOneAsync(l => l.IsSimilar(listing), listing);
            }
            catch (MongoWriteException)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> DeleteOneUnchecked(ListingEntity listing)
        {
            var result = await _context.Listings
                .DeleteOneAsync(l => l.IsSimilar(listing.ExchangeCode, listing.TickerSymbol));
            return result.DeletedCount > 0;
        }

        public async Task<bool> InsertOrReplaceOne(ListingEntity listing)
        {
            return listing.Sanitize().Validate().Count == 0
                && await ValidateReferences(listing)
                && await InsertOrReplaceOneUnchecked(listing);
        }

        public async Task<bool> DeleteOne(string exchangeCode, string ticker)
        {
            var listing = await FindOneByTicker(exchangeCode, ticker);
            return listing != null
                && await AssertNoReferences(listing)
                && await DeleteOneUnchecked(listing);
        }

        public async Task<ListingEntity> FindOneByTicker(string exchangeCode, string ticker)
        {
            var listing = await _context.Listings
                .Find(l => l.IsSimilar(exchangeCode, ticker))
                .FirstOrDefaultAsync();

            return listing;
        }
    }
}