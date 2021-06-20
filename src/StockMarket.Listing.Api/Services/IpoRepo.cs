using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Services
{
    public class IpoRepo
    {
        private readonly DatabaseContext _context;

        public IpoRepo(DatabaseContext context)
        {
            _context = context;
        }

        private async Task<bool> ValidateReferences(IpoEntity ipo)
        {
            // validate listing reference
            var listingFilter = ListingEntity.IsMatch(ipo.ExchangeCode, ipo.TickerSymbol);
            var listingExists = await _context.Listings.Find(listingFilter).AnyAsync();
            if (!listingExists) return false;

            return true;
        }

        private async Task<bool> AssertNoReferences(IpoEntity ipo)
        {
            // ipo does not have any dependents
            // so nothing for now
            await Task.Yield();
            return true;
        }
    
        private async Task<bool> InsertOneUnchecked(IpoEntity ipo)
        {
            try { await _context.Ipos.InsertOneAsync(ipo); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(IpoEntity ipo)
        {
            var filter = IpoEntity.IsMatch(ipo.ExchangeCode, ipo.TickerSymbol);
            try { await _context.Ipos.ReplaceOneAsync(filter, ipo); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> DeleteOneUnchecked(IpoEntity ipo)
        {
            var filter = IpoEntity.IsMatch(ipo.ExchangeCode, ipo.TickerSymbol);
            var result = await _context.Ipos.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    
        public async Task<bool> InsertOne(IpoEntity ipo)
        {
            return ipo.Sanitize().Validate().Count == 0
                && await ValidateReferences(ipo)
                && await InsertOneUnchecked(ipo);
        }

        public async Task<bool> ReplaceOne(IpoEntity ipo)
        {
            return ipo.Sanitize().Validate().Count == 0
                && await ValidateReferences(ipo)
                && await ReplaceOneUnchecked(ipo);
        }

        public async Task<bool> DeleteOne(string exchangeCode, string tickerSymbol)
        {
            var ipo = await FindOneByTicker(exchangeCode, tickerSymbol);
            return ipo != null
                && await AssertNoReferences(ipo)
                && await DeleteOneUnchecked(ipo);
        }

        public async Task<List<IpoEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);
            return await _context.Ipos
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<IpoEntity> FindOneByTicker(string exchangeCode, string tickerSymbol)
        {
            // case insensitive
            exchangeCode = exchangeCode.ToUpper();
            tickerSymbol = tickerSymbol.ToUpper();
            var filter = IpoEntity.IsMatch(exchangeCode, tickerSymbol);
            return await _context.Ipos.Find(filter).FirstOrDefaultAsync();
        }
    }
}