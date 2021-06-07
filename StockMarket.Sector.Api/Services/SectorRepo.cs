using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Sector.Api.Models;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Services
{
    public class SectorRepo
    {
        private readonly DatabaseContext _context;
        private readonly EventBus _events;

        public SectorRepo(DatabaseContext context, EventBus events)
        {
            _context = context;
            _events = events;
        }

        public async Task<bool> InsertOrReplaceOne(SectorEntity sector)
        {
            // sanitize and validate sector model
            var validationErrors = sector.Sanitize().Validate();
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

            // broadcast sector creation event
            var creationEvent = SectorCreationEvent.FromEntity(sector);
            await _events.Publish<ISectorIntegrationEvent>(creationEvent);

            return true;
        }

        public async Task<bool> DeleteOne(string code)
        {
            // delete sector from database
            var dbSector = await _context.Sector.FindOneAndDeleteAsync(e => e.SectorCode == code);
            if (dbSector == null) return false;

            // broadcast sector deletion event
            var deletionEvent = SectorDeletionEvent.FromId(dbSector.Id);
            await _events.Publish<ISectorIntegrationEvent>(deletionEvent);

            return true;
        }

        public async Task<List<SectorEntity>> Enumerate(int page = 1, int count = 10)
        {
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 25);
            // TODO: add pagination feature
            var dbSectorList = await _context.Sector.Find(x => true).ToListAsync();
            return dbSectorList;
        }

        public async Task<SectorEntity> FindOneByCode(string code)
        {
            var dbSector = await _context.Sector.Find(e => e.SectorCode == code).FirstOrDefaultAsync();
            return dbSector;
        }
    }
}