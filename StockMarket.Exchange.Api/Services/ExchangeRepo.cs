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

        public async Task<bool> InsertOrReplaceOne(ExchangeEntity exchange)
        {
            // validate exchange model
            var validationErrors = exchange.Validate();
            if (validationErrors.Count > 0) return false;
    
            // check if entry exists already
            var exchangeExists = await _context.Exchange
                .Find(e => e.ExchangeCode == exchange.ExchangeCode)
                .AnyAsync();

            if (!exchangeExists)
            {
                // try inserting exchange model
                // this fails if any constraint is violated
                try { await _context.Exchange.InsertOneAsync(exchange); }
                catch (MongoWriteException) { return false; }
            }
            else
            {
                // try replacing exchange model
                // this fails if any constraint is violated
                var code = exchange.ExchangeCode;
                try { await _context.Exchange.ReplaceOneAsync(e => e.ExchangeCode == code, exchange); }
                catch (MongoWriteException) { return false; }
            }

            // broadcast created event
            var creationEvent = ExchangeCreationEvent.FromEntity(exchange);
            await _events.Publish<IExchangeIntegrationEvent>(creationEvent);

            return true;
        }

        public async Task<bool> DeleteOne(string code)
        {
            // delete exchange from database
            var dbExchange = await _context.Exchange.FindOneAndDeleteAsync(e => e.ExchangeCode == code);
            if (dbExchange == null) return false;

            // broadcast exchange deletion event
            var deletionEvent = ExchangeDeletionEvent.FromId(dbExchange.Id);
            await _events.Publish<IExchangeIntegrationEvent>(deletionEvent);

            return true;
        }

        public async Task<List<ExchangeEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);

            // TODO: implement pagination
            var dbExchangeList = await _context.Exchange.Find(x => true).ToListAsync();
            return dbExchangeList;
        }

        public async Task<ExchangeEntity> FindOneByCode(string code)
        {
            var dbExchange = await _context.Exchange.Find(e => e.ExchangeCode == code).FirstOrDefaultAsync();
            return dbExchange;
        }
    }
}