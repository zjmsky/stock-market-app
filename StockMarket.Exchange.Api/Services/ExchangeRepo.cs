using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace StockMarket.Exchange.Api.Services
{
    public class ExchangeRepo
    {
        private readonly DatabaseContext _context;

        public ExchangeRepo(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertOne(Entities.Exchange exchange)
        {
            var validationErrors = exchange.Validate();
            if (validationErrors.Count > 0)
                return false;

            exchange.Id = ObjectId.Empty;
            try { await _context.Exchange.InsertOneAsync(exchange); }
            catch (MongoWriteException) { return false; }

            return true;
        }

        public async Task<bool> ReplaceOne(Entities.Exchange exchange)
        {
            var validationErrors = exchange.Validate();
            if (validationErrors.Count > 0)
                return false;

            try { await _context.Exchange.ReplaceOneAsync(e => e.Id == exchange.Id, exchange); }
            catch (MongoWriteException) { return false; }

            return true;
        }

        public async Task<bool> DeleteOne(string id)
        {
            var exchangeId = new ObjectId(id);
            var result = await _context.Exchange.DeleteOneAsync(e => e.Id == exchangeId);
            return result.DeletedCount > 0;
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