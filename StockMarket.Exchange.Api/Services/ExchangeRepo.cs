using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Data;

namespace StockMarket.Exchange.Api.Services
{
    public class ExchangeRepo
    {
        private readonly AppDbContext _context;

        public ExchangeRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertOneAsync(Models.Exchange document)
        {
            Models.ExchangeProcessor.Validate(document);
            Models.ExchangeProcessor.Sanitize(document);
            await _context.Exchange.InsertOneAsync(document);
        }

        public async Task<List<Models.Exchange>> ListPageAsync()
        {
            return await _context.Exchange.Find(x => true).ToListAsync();
        }

        public async Task<Models.Exchange> FindOneAsync(Func<Models.Exchange, bool> filter)
        {
            return await _context.Exchange.Find(x => filter(x)).FirstAsync();
        }
    }
}