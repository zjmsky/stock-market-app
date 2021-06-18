using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Services
{
    public class ExchangeRepo
    {
        private readonly DatabaseContext _context;

        public ExchangeRepo(DatabaseContext context)
        {
            _context = context;
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

        public async Task<bool> InsertOrReplaceOneUnchecked(ExchangeEntity exchange)
        {
            return await InsertOneUnchecked(exchange) || await ReplaceOneUnchecked(exchange);
        }

        public async Task<bool> DeleteOneUnchecked(ExchangeEntity exchange)
        {
            var code = exchange.ExchangeCode;
            var result = await _context.Exchanges.DeleteOneAsync(e => e.ExchangeCode == code);
            return result.DeletedCount > 0;
        }

        public async Task<ExchangeEntity> FindOneByCode(string code)
        {
            return await _context.Exchanges
                .Find(e => e.ExchangeCode == code)
                .FirstOrDefaultAsync();
        }
    }
}