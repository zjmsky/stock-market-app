using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
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

        public async Task<bool> InsertOne(Entities.Exchange exchange)
        {
            // validate exchange model
            var validationErrors = exchange.Validate();
            if (validationErrors.Count > 0) return false;

            // try inserting into database
            // this fails if uniqueness is violated
            exchange.Id = ObjectId.Empty;
            try { await _context.Exchange.InsertOneAsync(exchange); }
            catch (MongoWriteException) { return false; }

            // broadcast exchange created event
            var createdEvent = new ExchangeCreatedEvent
            {
                Id = exchange.Id.ToString(),
                ExchangeCode = exchange.ExchangeCode,
                Name = exchange.Name
            };
            await _events.Publish<IExchangeIntegrationEvent>(createdEvent);

            return true;
        }

        public async Task<bool> ReplaceOne(Entities.Exchange exchange)
        {
            // validate exchange model
            var validationErrors = exchange.Validate();
            if (validationErrors.Count > 0)
                return false;

            // try inserting into database
            // this fails if uniqueness is violated
            try { await _context.Exchange.ReplaceOneAsync(e => e.Id == exchange.Id, exchange); }
            catch (MongoWriteException) { return false; }

            // broadcast exchange created event
            var createdEvent = new ExchangeCreatedEvent
            {
                Id = exchange.Id.ToString(),
                ExchangeCode = exchange.ExchangeCode,
                Name = exchange.Name
            };
            await _events.Publish<IExchangeIntegrationEvent>(createdEvent);

            return true;
        }

        public async Task<bool> DeleteOne(string id)
        {
            // delete exchange from database
            var exchangeId = new ObjectId(id);
            var dbResult = await _context.Exchange.DeleteOneAsync(e => e.Id == exchangeId);
            if (dbResult.DeletedCount == 0) return false;

            // broadcast exchange deleted event
            var deletedEvent = new ExchangeDeletedEvent { Id = id };
            await _events.Publish<IExchangeIntegrationEvent>(deletedEvent);

            return true;
        }

        public async Task<List<Entities.Exchange>> ListPage(int page = 1, int count = 10)
        {
            // TODO: add pagination feature
            var dbExchangeList = await _context.Exchange.Find(x => true).ToListAsync();
            return dbExchangeList;
        }

        public async Task<Entities.Exchange> FindOneById(string id)
        {
            var exchangeId = new ObjectId(id);
            var dbExchange = await _context.Exchange.Find(e => e.Id == exchangeId).FirstOrDefaultAsync();
            return dbExchange;
        }
    }
}