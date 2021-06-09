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

        public async Task<bool> InsertOrReplaceOne(ListingEntity listing)
        {
            // validate listing entity
            var validationErrors = listing.Validate();
            if (validationErrors.Count > 0) return false;

            // ensure corresponding company exists
            var companyExists = await _context.Companies
                .Find(c => c.CompanyCode == listing.CompanyCode)
                .AnyAsync();
            if (!companyExists) return false;

            // ensure corresponding exchange exists
            var exchangeExists = await _context.Exchanges
                .Find(e => e.ExchangeCode == listing.ExchangeCode)
                .AnyAsync();
            if (!exchangeExists) return false;

            var opResult = await InsertOrReplaceOneUnchecked(listing);
            if (!opResult) return false;

            return true;
        }

        public async Task<bool> DeleteOne(string exchangeCode, string ticker)
        {
            // delete listing entity
            var success = await _context.Listings
                .DeleteOneAsync(l => l.IsSimilar(exchangeCode, ticker));
            return success.DeletedCount > 0;
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