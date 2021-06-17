using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Models;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Services
{
    public class ExchangeRepo
    {
        private readonly DatabaseContext _context;
        private readonly EventBus _events;

        public ExchangeRepo(DatabaseContext context, EventBus events)
        {
            _context = context;
            _events = events;
        }

        private async Task<bool> ValidateReferences(ExchangeEntity exchange)
        {
            // exchange does not depend on any entity
            // so nothing for now
            await Task.Yield();
            return true;
        }

        private async Task<bool> AssertNoReferences(ExchangeEntity exchange)
        {
            // assert no reference in listing collection
            var listingExists = await _context.Listings
                .Find(l => l.ExchangeCode == exchange.ExchangeCode)
                .AnyAsync();
            if (listingExists) return false;

            return true;
        }

        private async Task<bool> InsertOneUnchecked(ExchangeEntity exchange)
        {
            try { await _context.Exchanges.InsertOneAsync(exchange); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(ExchangeEntity exchange)
        {
            var code = exchange.ExchangeCode;
            try { await _context.Exchanges.ReplaceOneAsync(e => e.ExchangeCode == code, exchange); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> DeleteOneUnchecked(ExchangeEntity exchange)
        {
            var code = exchange.ExchangeCode;
            var result = await _context.Exchanges.DeleteOneAsync(e => e.ExchangeCode == code);
            return result.DeletedCount > 0;
        }

        private async Task<bool> PublishUpdation(ExchangeEntity exchange)
        {
            var updationEvent = ExchangeIntegrationEvent.Update(exchange);
            await _events.Publish<ExchangeIntegrationEvent>(updationEvent);
            return true;
        }

        private async Task<bool> PublishDeletion(ExchangeEntity exchange)
        {
            var deletionEvent = ExchangeIntegrationEvent.Delete(exchange);
            await _events.Publish<ExchangeIntegrationEvent>(deletionEvent);
            return true;
        }

        public async Task<bool> InsertOne(ExchangeEntity exchange)
        {
            return exchange.Sanitize().Validate().Count == 0
                && await ValidateReferences(exchange)
                && await InsertOneUnchecked(exchange)
                && await PublishUpdation(exchange);
        }

        public async Task<bool> ReplaceOne(ExchangeEntity exchange)
        {
            return exchange.Sanitize().Validate().Count == 0
                && await ValidateReferences(exchange)
                && await ReplaceOneUnchecked(exchange)
                && await PublishUpdation(exchange);
        }

        public async Task<bool> DeleteOne(string code)
        {
            var exchange = await FindOneByCode(code);
            return exchange != null
                && await AssertNoReferences(exchange)
                && await DeleteOneUnchecked(exchange)
                && await PublishDeletion(exchange);
        }

        public async Task<List<ExchangeEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);
            return await _context.Exchanges
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<ExchangeEntity> FindOneByCode(string code)
        {
            // case insensitive
            code = code.ToUpper();
            return await _context.Exchanges
                .Find(e => e.ExchangeCode == code)
                .FirstOrDefaultAsync();
        }
    }
}