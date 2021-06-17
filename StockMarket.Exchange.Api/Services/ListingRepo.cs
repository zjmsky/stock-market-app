using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Services
{
    public class ListingRepo
    {
        private readonly DatabaseContext _context;

        public ListingRepo(DatabaseContext context)
        {
            _context = context;
        }

        private async Task<bool> InsertOneUnchecked(ListingEntity listing)
        {
            try { await _context.Listings.InsertOneAsync(listing); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(ListingEntity listing)
        {
            try { await _context.Listings.ReplaceOneAsync(l => l.IsMatch(listing), listing); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        public async Task<bool> InsertOrReplaceOneUnchecked(ListingEntity listing)
        {
            return await InsertOneUnchecked(listing) || await ReplaceOneUnchecked(listing);
        }

        public async Task<bool> DeleteOneUnchecked(ListingEntity listing)
        {
            var result = await _context.Listings.DeleteOneAsync(l => l.IsMatch(listing));
            return result.DeletedCount > 0;
        }

        public async Task<ListingEntity> FindOneByTicker(string exchangeCode, string tickerSymbol)
        {
            return await _context.Listings
                .Find(l => l.IsMatch(exchangeCode, tickerSymbol))
                .FirstOrDefaultAsync();
        }
    }
}