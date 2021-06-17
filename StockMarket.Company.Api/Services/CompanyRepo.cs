using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Company.Api.Entities;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class CompanyRepo
    {
        private readonly DatabaseContext _context;
        private readonly EventBus _events;

        public CompanyRepo(DatabaseContext context, EventBus events)
        {
            _context = context;
            _events = events;
        }

        private async Task<bool> ValidateReferences(CompanyEntity company)
        {
            // validate sector reference
            var sectorExists = await _context.Sectors
                .Find(s => s.SectorCode == company.SectorCode)
                .AnyAsync();
            if (!sectorExists) return false;

            return true;
        }

        private async Task<bool> AssertNoReferences(CompanyEntity company)
        {
            // assert no reference in listings collection
            var listingExists = await _context.Listings
                .Find(l => l.CompanyCode == company.CompanyCode)
                .AnyAsync();
            if (listingExists) return false;

            return true;
        }

        private async Task<bool> InsertOneUnchecked(CompanyEntity company)
        {
            try { await _context.Companies.InsertOneAsync(company); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(CompanyEntity company)
        {
            var code = company.CompanyCode;
            try { await _context.Companies.ReplaceOneAsync(c => c.CompanyCode == code, company); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> DeleteOneUnchecked(CompanyEntity company)
        {
            var code = company.CompanyCode;
            var result = await _context.Companies.DeleteOneAsync(c => c.CompanyCode == code);
            return result.DeletedCount > 0;
        }

        private async Task<bool> PublishUpdation(CompanyEntity company)
        {
            var updationEvent = CompanyIntegrationEvent.Update(company);
            await _events.Publish<CompanyIntegrationEvent>(updationEvent);
            return true;
        }

        private async Task<bool> PublishDeletion(CompanyEntity company)
        {
            var deletionEvent = CompanyIntegrationEvent.Delete(company);
            await _events.Publish<CompanyIntegrationEvent>(deletionEvent);
            return true;
        }

        public async Task<bool> InsertOne(CompanyEntity company)
        {
            return company.Sanitize().Validate().Count == 0
                && await ValidateReferences(company)
                && await InsertOneUnchecked(company)
                && await PublishUpdation(company);
        }

        public async Task<bool> ReplaceOne(CompanyEntity company)
        {
            return company.Sanitize().Validate().Count == 0
                && await ValidateReferences(company)
                && await ReplaceOneUnchecked(company)
                && await PublishUpdation(company);
        }

        public async Task<bool> DeleteOne(string companyCode)
        {
            var company = await FindOneByCode(companyCode);
            return company != null
                && await AssertNoReferences(company)
                && await DeleteOneUnchecked(company)
                && await PublishDeletion(company);
        }

        public async Task<List<CompanyEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);
            return await _context.Companies
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<CompanyEntity> FindOneByCode(string code)
        {
            // case insensitive
            code = code.ToUpper();
            return await _context.Companies
                .Find(c => c.CompanyCode == code)
                .FirstOrDefaultAsync();
        }
    }
}