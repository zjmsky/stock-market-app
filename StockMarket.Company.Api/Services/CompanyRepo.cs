using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using StockMarket.Company.Api.Data;

namespace StockMarket.Company.Api.Services
{
    public class CompanyRepo
    {
        private readonly AppDbContext _context;

        public CompanyRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertOne(Entities.Company company)
        {
            // TODO: add validations

            company.Id = ObjectId.Empty;
            try { await _context.Companies.InsertOneAsync(company); }
            catch (MongoWriteException) { return false; }

            return true;
        }

        public async Task<bool> UpdateOne(Entities.Company company)
        {
            // TODO: add validations
            
            try { await _context.Companies.ReplaceOneAsync(c => c.Id == company.Id, company); }
            catch (MongoWriteException) { return false; }

            return true;
        }

        public async Task<List<Entities.Company>> ListPage(int page = 1, int count = 10)
        {
            return await _context.Companies.Find(x => true).ToListAsync();
        }

        public async Task<Entities.Company> FindOneByTicker(string ticker, string exchangeCode = "")
        {
            var listings = await _context.Listings
                .Find(l => l.TickerSymbol == ticker)
                .ToListAsync();

            if (exchangeCode != "")
                listings.RemoveAll(l => l.ExchangeCode != exchangeCode);
            
            if (listings.Count == 0)
                return null;

            var company = await _context.Companies
                .Find(c => c.Id == listings[0].CompanyId)
                .FirstOrDefaultAsync();            

            if (company == null)
                return null;

            return company;
        }
    }
}