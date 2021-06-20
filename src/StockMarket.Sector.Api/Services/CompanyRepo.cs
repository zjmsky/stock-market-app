using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Services
{
    public class CompanyRepo
    {
        private readonly DatabaseContext _context;

        public CompanyRepo(DatabaseContext context)
        {
            _context = context;
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

        public async Task<bool> InsertOrReplaceOneUnchecked(CompanyEntity company)
        {
            return await InsertOneUnchecked(company) || await ReplaceOneUnchecked(company);
        }

        public async Task<bool> DeleteOneUnchecked(CompanyEntity company)
        {
            var code = company.CompanyCode;
            var result = await _context.Companies.DeleteOneAsync(c => c.CompanyCode == code);
            return result.DeletedCount > 0;
        }

        public async Task<CompanyEntity> FindOneByCode(string companyCode)
        {
            return await _context.Companies
                .Find(c => c.CompanyCode == companyCode)
                .FirstOrDefaultAsync();
        }
    }
}