using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Models;

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

        public async Task<bool> InsertOrReplaceOne(Entities.Exchange exchange)
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
            var createdEvent = new ExchangeCreatedIntegrationEvent(exchange);
            await _events.Publish<IExchangeIntegrationEvent>(createdEvent);

            return true;
        }

        public async Task<bool> DeleteOne(string code)
        {
            // delete exchange from database
            var dbResult = await _context.Exchange.DeleteOneAsync(e => e.ExchangeCode == code);
            if (dbResult.DeletedCount == 0) return false;

            // broadcast exchange deleted event
            var deletedEvent = new ExchangeDeletedIntegrationEvent(code);
            await _events.Publish<IExchangeIntegrationEvent>(deletedEvent);

            return true;
        }

        public async Task<List<Entities.Exchange>> ListPage(int page = 1, int count = 10)
        {
            // TODO: add pagination feature
            var dbExchangeList = await _context.Exchange.Find(x => true).ToListAsync();
            return dbExchangeList;
        }

        public async Task<Entities.Exchange> FindOneByCode(string code)
        {
            var dbExchange = await _context.Exchange.Find(e => e.ExchangeCode == code).FirstOrDefaultAsync();
            return dbExchange;
        }
    }
}