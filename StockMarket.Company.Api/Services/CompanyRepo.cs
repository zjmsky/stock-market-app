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

        private async Task<bool> InsertOrReplaceOneUnchecked(CompanyEntity company)
        {
            var code = company.CompanyCode;
            var companyExists = await _context.Companies
                .Find(c => c.CompanyCode == code)
                .AnyAsync();

            try
            {
                if (!companyExists)
                    await _context.Companies.InsertOneAsync(company);
                else
                    await _context.Companies.ReplaceOneAsync(c => c.CompanyCode == code, company);
            }
            catch (MongoWriteException)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> DeleteOneUnchecked(CompanyEntity company)
        {
            var result = await _context.Companies.DeleteOneAsync(c => c.Id == company.Id);
            return result.DeletedCount > 0;
        }

        private async Task<bool> PublishCreation(CompanyEntity company)
        {
            var creationEvent = CompanyCreationEvent.FromEntity(company);
            await _events.Publish<ICompanyIntegrationEvent>(creationEvent);
            return true;
        }

        private async Task<bool> PublishDeletion(CompanyEntity company)
        {
            var deletionEvent = CompanyDeletionEvent.FromId(company.Id);
            await _events.Publish<ICompanyIntegrationEvent>(deletionEvent);
            return true;
        }

        public async Task<bool> InsertOrReplaceOne(CompanyEntity company)
        {
            return company.Sanitize().Validate().Count == 0
                && await ValidateReferences(company)
                && await InsertOrReplaceOneUnchecked(company)
                && await PublishCreation(company);
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

            // TODO: implement pagination
            var companyList = await _context.Companies.Find(x => true).ToListAsync();
            return companyList;
        }

        public async Task<CompanyEntity> FindOneByCode(string companyCode)
        {
            var company = await _context.Companies
                .Find(c => c.CompanyCode == companyCode)
                .FirstOrDefaultAsync();

            return company;
        }

        public async Task<CompanyEntity> FindOneByTicker(string exchangeCode, string ticker)
        {
            var listing = await _context.Listings
                .Find(l => l.IsSimilar(exchangeCode, ticker))
                .FirstOrDefaultAsync();

            if (listing == null)
                return null;

            // broken references throw exceptions
            var company = await _context.Companies
                .Find(c => c.CompanyCode == listing.CompanyCode)
                .FirstAsync();

            return company;
        }
    }
}