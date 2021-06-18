using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Listing.Api.Models;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Services
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
            // assert no reference in ipo collection
            var ipoExists = await _context.Ipos
                .Find(i => i.IsMatch(listing))
                .AnyAsync();
            if (ipoExists) return false;

            return true;
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

        private async Task<bool> DeleteOneUnchecked(ListingEntity listing)
        {
            var result = await _context.Listings.DeleteOneAsync(l => l.IsMatch(listing));
            return result.DeletedCount > 0;
        }
    
        private async Task<bool> PublishUpdation(ListingEntity listing)
        {
            var updationEvent = ListingIntegrationEvent.Update(listing);
            await _events.Publish<ListingIntegrationEvent>(updationEvent);
            return true;
        }

        private async Task<bool> PublishDeletion(ListingEntity listing)
        {
            var deletionEvent = ListingIntegrationEvent.Delete(listing);
            await _events.Publish<ListingIntegrationEvent>(deletionEvent);
            return true;
        }
    
        public async Task<bool> InsertOne(ListingEntity listing)
        {
            return listing.Sanitize().Validate().Count == 0
                && await ValidateReferences(listing)
                && await InsertOneUnchecked(listing)
                && await PublishUpdation(listing);
        }

        public async Task<bool> ReplaceOne(ListingEntity listing)
        {
            return listing.Sanitize().Validate().Count == 0
                && await ValidateReferences(listing)
                && await ReplaceOneUnchecked(listing)
                && await PublishUpdation(listing);
        }

        public async Task<bool> DeleteOne(string exchangeCode, string tickerSymbol)
        {
            var listing = await FindOneByTicker(exchangeCode, tickerSymbol);
            return listing != null
                && await AssertNoReferences(listing)
                && await DeleteOneUnchecked(listing)
                && await PublishDeletion(listing);
        }

        public async Task<List<ListingEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);
            return await _context.Listings
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<ListingEntity> FindOneByTicker(string exchangeCode, string tickerSymbol)
        {
            // case insensitive
            exchangeCode = exchangeCode.ToUpper();
            tickerSymbol = tickerSymbol.ToUpper();
            return await _context.Listings
                .Find(l => l.IsMatch(exchangeCode, tickerSymbol))
                .FirstOrDefaultAsync();
        }
    }
}