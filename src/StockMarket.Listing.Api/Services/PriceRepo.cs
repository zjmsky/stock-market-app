using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Services
{
    public class PriceRepo
    {
        private readonly DatabaseContext _context;

        public PriceRepo(DatabaseContext context)
        {
            _context = context;
        }

        private async Task<bool> ValidateReferences(PriceEntity price)
        {
            // validate listing reference
            var listingFilter = ListingEntity.IsMatch(price.ExchangeCode, price.TickerSymbol);
            var listingExists = await _context.Listings.Find(listingFilter).AnyAsync();
            if (!listingExists) return false;

            return true;
        }

        private async Task<bool> AssertNoReferences(PriceEntity price)
        {
            // price does not have any dependents
            // so nothing for now
            await Task.Yield();
            return true;
        }
    
        private async Task<bool> InsertOneUnchecked(PriceEntity price)
        {
            try { await _context.Prices.InsertOneAsync(price); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(PriceEntity price)
        {
            var filter = PriceEntity.IsOverlap(price);
            try { await _context.Prices.ReplaceOneAsync(filter, price); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        public async Task<bool> InsertOne(PriceEntity price)
        {
            return price.Sanitize().Validate().Count == 0
                && await ValidateReferences(price)
                && await InsertOneUnchecked(price);
        }

        public async Task<bool> ReplaceOne(PriceEntity price)
        {
            return price.Sanitize().Validate().Count == 0
                && await ValidateReferences(price)
                && await ReplaceOneUnchecked(price);
        }

        public async Task<List<PriceEntity>> Enumerate(
            string exchange,
            string ticker,
            DateTime fromTime,
            DateTime toTime
        )
        {
            return await _context.Prices
                .Find(p =>
                    p.TickerSymbol == ticker &&
                    p.ExchangeCode == exchange &&
                    p.Time >= fromTime &&
                    p.Time <= toTime)
                .ToListAsync();
        }
    }
}