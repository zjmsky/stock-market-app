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
            // nothing for now
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
            try { await _context.Exchange.InsertOneAsync(exchange); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(ExchangeEntity exchange)
        {
            var code = exchange.ExchangeCode;
            try { await _context.Exchange.ReplaceOneAsync(e => e.ExchangeCode == code, exchange); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> DeleteOneUnchecked(ExchangeEntity exchange)
        {
            var code = exchange.ExchangeCode;
            var result = await _context.Exchange.DeleteOneAsync(e => e.ExchangeCode == code);
            return result.DeletedCount > 0;
        }

        private async Task<bool> PublishCreation(ExchangeEntity exchange)
        {
            var creationEvent = ExchangeCreationEvent.FromEntity(exchange);
            await _events.Publish<IExchangeIntegrationEvent>(creationEvent);
            return true;
        }

        private async Task<bool> PublishDeletion(ExchangeEntity exchange)
        {
            var deletionEvent = ExchangeDeletionEvent.FromId(exchange.Id);
            await _events.Publish<IExchangeIntegrationEvent>(deletionEvent);
            return true;
        }

        public async Task<bool> InsertOne(ExchangeEntity exchange)
        {
            return exchange.Sanitize().Validate().Count == 0
                && await ValidateReferences(exchange)
                && await InsertOneUnchecked(exchange)
                && await PublishCreation(exchange);
        }

        public async Task<bool> ReplaceOne(ExchangeEntity exchange)
        {
            return exchange.Sanitize().Validate().Count == 0
                && await ValidateReferences(exchange)
                && await ReplaceOneUnchecked(exchange)
                && await PublishCreation(exchange);
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

            return await _context.Exchange
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<ExchangeEntity> FindOneByCode(string code)
        {
            code = code.ToUpper(); // case insensitive
            return await _context.Exchange
                .Find(e => e.ExchangeCode == code)
                .FirstOrDefaultAsync();
        }
    }
}