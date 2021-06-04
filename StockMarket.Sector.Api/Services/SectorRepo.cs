using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace StockMarket.Sector.Api.Services
{
    public class SectorRepo
    {
        private readonly DatabaseContext _context;

        public SectorRepo(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertOrReplaceOne(Entities.Sector sector)
        {
            // validate sector model
            var validationErrors = sector.Validate();
            if (validationErrors.Count > 0) return false;
    
            // check if entry exists already
            var sectorExists = await _context.Sector
                .Find(e => e.SectorCode == sector.SectorCode)
                .AnyAsync();

            if (!sectorExists)
            {
                // try inserting sector model
                // this fails if any constraint is violated
                try { await _context.Sector.InsertOneAsync(sector); }
                catch (MongoWriteException) { return false; }
            }
            else
            {
                // try replacing sector model
                // this fails if any constraint is violated
                var code = sector.SectorCode;
                try { await _context.Sector.ReplaceOneAsync(e => e.SectorCode == code, sector); }
                catch (MongoWriteException) { return false; }
            }

            return true;
        }

        public async Task<bool> DeleteOne(string code)
        {
            // delete sector from database
            var dbResult = await _context.Sector.DeleteOneAsync(e => e.SectorCode == code);
            if (dbResult.DeletedCount == 0) return false;

            return true;
        }

        public async Task<List<Entities.Sector>> ListPage(int page = 1, int count = 10)
        {
            // TODO: add pagination feature
            var dbSectorList = await _context.Sector.Find(x => true).ToListAsync();
            return dbSectorList;
        }

        public async Task<Entities.Sector> FindOneByCode(string code)
        {
            var dbSector = await _context.Sector.Find(e => e.SectorCode == code).FirstOrDefaultAsync();
            return dbSector;
        }
    }
}