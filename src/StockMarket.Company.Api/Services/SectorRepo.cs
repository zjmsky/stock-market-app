using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Services
{
    public class SectorRepo
    {
        private readonly DatabaseContext _context;

        public SectorRepo(DatabaseContext context)
        {
            _context = context;
        }

        private async Task<bool> InsertOneUnchecked(SectorEntity sector)
        {
            try { await _context.Sectors.InsertOneAsync(sector); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(SectorEntity sector)
        {
            var code = sector.SectorCode;
            try { await _context.Sectors.ReplaceOneAsync(s => s.SectorCode == code, sector); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        public async Task<bool> InsertOrReplaceOneUnchecked(SectorEntity sector)
        {
            return await InsertOneUnchecked(sector) || await ReplaceOneUnchecked(sector);
        }

        public async Task<bool> DeleteOneUnchecked(SectorEntity sector)
        {
            var code = sector.SectorCode;
            var result = await _context.Sectors.DeleteOneAsync(s => s.SectorCode == code);
            return result.DeletedCount > 0;
        }

        public async Task<SectorEntity> FindOneByCode(string code)
        {
            return await _context.Sectors
                .Find(s => s.SectorCode == code)
                .FirstOrDefaultAsync();
        }
    }
}